import pandas as pd
from pandas import DataFrame
import spacy
import re
from spacy.tokens import Token

#refer to nlp.py for more info and easy run and viewing result at each step
from jobs.utils.nlp_utils import filter_pos, filter_some_nouns, filter_stop_words, map_list_tokens_to_sublists_tokens, map_to_one_string, filter_simple_structure_sublists
nlp = spacy.load('en_core_web_md')
def transform_preprocess_usage_instruction_df_to_sublists_tokens(df: DataFrame) -> list[list[Token]]:
    """
    Convert each unstructure usage instruction to list of sublist of tokens(been preprocessed
    like remove space, punctuation, etc and filter by pos tag spacy))
    .Return list of sublists(which contain list of tokens) for each usage instruction)"""
    result = []
    for _, df_row in df.iterrows():
        usage_instruction = df_row['ProductUsageInstruction']
        usage_instruction = re.sub(r'[^\w\s]', '', usage_instruction)
        usage_instruction = re.sub(r'^\d?', '', usage_instruction)
        usage_instruction = usage_instruction.strip()

        doc = nlp(usage_instruction)
        tokens_with_pos = [token for token in doc]
        token_filtered_noun = list(filter(filter_some_nouns, tokens_with_pos))
        tokens_filtered_by_pos = list(filter(filter_pos, token_filtered_noun))
        # DO NOT LEMMATIZE, WE NEED TO RETAIN THE GRAMMER STRUCTURE UNTIL CONVERT TO SUBLISTS
        tokens_lowered = list(map(lambda token: token.text.lower(), tokens_filtered_by_pos))

        doc = nlp(' '.join(tokens_lowered))
        token_list = [token for token in doc]
        sublists = map_list_tokens_to_sublists_tokens(token_list)

        sublists = filter_simple_structure_sublists(sublists)

        result.append(sublists)
        #for debugging only, print grammar pos and dep tag for understanding
        for sublist in sublists:
            # grammar_tokens_dep = [token.dep_ for token in sublist]
            # grammar_tokens_pos = [token.pos_ for token in sublist]
            # print("Grammer token Dep to string: ", map_to_one_string(grammar_tokens_dep), '\n')
            # print("Grammer token Pos to string: ", map_to_one_string(grammar_tokens_pos), '\n')
            # print("Grammar token Dep: ",grammar_tokens_dep, '\n')
            # print("Grammar token Pos: ",grammar_tokens_pos, '\n')
            # print("=========================================================================================================================")
            pass
    return result

from jobs.utils.nlp_utils import map_to_lemmatized_spacy, map_token_to_stemmed_nltk
def transform_lemm_stem_sublists(sublists_list: list[list[Token]]) -> list[list[str]]:
    """Convert each sublist of tokens to sublist of lemmatized and stemmed tokens"""
    result = []
    for sublists in sublists_list:
        sublists = list(map(lambda sublist: list(map(map_to_lemmatized_spacy, sublist)), sublists))
        sublists = list(map(lambda sublist: list(map(map_token_to_stemmed_nltk, sublist)), sublists))
        result.append(sublists)
    return result

def transform_sublists_to_reduced_usage_instruction_df(sublists_list: list[list[list[str]]]) -> DataFrame:
    """Convert list of sublists of tokens to dataframe with columns: UsageInstructionTypeId, UsageInstructionTypeName"""
    # make a dict with key is a usage_instruction, value is count of appearance
    dict = {}
    for sublists in sublists_list:
        for sublist in sublists:
            sublist_str = map_to_one_string(sublist)
            if sublist_str in dict:
                dict[sublist_str] += 1
            else:
                dict[sublist_str] = 1
    # sort dict by value of appearance
    dict = {k: v for k, v in sorted(dict.items(), key=lambda item: item[1], reverse=True)}

    # Then reduce the dict by removing LATTER (since its sorted by appearance so the latter is less appeared) similar keys by vector similarity
    keys_list = []
    keys = dict.keys()
    for key in keys:
        keys_list.append(key)

    # using bubble sort to compare each key with other keys in similarity
    keys_to_remove_set = set()
    len_key_list = len(keys_list)
    for i in range(len_key_list):
        for j in range(i+1, len_key_list):
            doc_one = nlp(keys_list[i])
            doc_two = nlp(keys_list[j])
            similarity = doc_two.similarity(doc_one)
            if (similarity > 0.89): #TODO: change this threshold later
                remove_key = keys_list[j]
                keys_to_remove_set.add(remove_key)

    for key_to_remove in keys_to_remove_set:
        del dict[key_to_remove]

    # remove key with count = 1, which means it only appear once for a product and too specific for a usage instruction type
    keys_to_remove_set.clear()
    for key, count_value in dict.items():
        if count_value == 1:
            keys_to_remove_set.add(key)

    for key_to_remove in keys_to_remove_set:
        del dict[key_to_remove]

    # convert dict to dataframe (with some more default usage instruction type)
    usage_instruction_type_dict = {
        0: "no usage instruction",
        1: "too special usage instruction with steps",
    }
    idx = 2
    for usage_instruction_type in dict.keys():
        usage_instruction_type_dict[idx] = usage_instruction_type
        idx += 1

    usage_instruction_type_df = pd.DataFrame.from_dict(usage_instruction_type_dict, orient='index', columns=['UsageInstructionTypeName'])
    usage_instruction_type_df.to_csv('resources/reduced_usage_instruction_type.csv', index_label='UsageInstructionTypeId')
    print("Usage instruction type dict: ", usage_instruction_type_dict, '\n')
    return usage_instruction_type_df

from jobs.utils.nlp_utils import map_to_processed_tokens
def transform_to_usage_instruction_df_with_topic_and_word_representation(usage_instruction_df: DataFrame, best_topic_numer: int = 8) -> DataFrame:
    """
    Convert usage instruction dataframe to bag of words dataframe
    Then train LDA model on the bag of words dataframe to get topic and word representation for each topic
    Return dataframe with columns: TopicId, TopicWords as result"""
    processed_tokens_lists = []
    for usage_instruction_text in usage_instruction_df['ProductUsageInstruction']:
        processed_tokens_lists.append(map_to_processed_tokens(usage_instruction_text))
        # print(processed_tokens_lists[-1])

    from gensim.corpora.dictionary import Dictionary
    dictionary = Dictionary(processed_tokens_lists)
    # fine-tune the dictionary by removing words that are too rare or too common from the dictionary:
    dictionary.filter_extremes(no_below=3, no_above=0.5, keep_n=1000)

    # convert dictionary to bag of words
    bow_corpus = [dictionary.doc2bow(token_list) for token_list in processed_tokens_lists]

    from gensim.models import LdaMulticore
    # train the model on the corpus
    lda_model = LdaMulticore(corpus=bow_corpus, id2word=dictionary\
                        , iterations=10, num_topics=best_topic_numer, workers = 4, passes=10\
                        , random_state=100, per_word_topics=True)

    topic_dict = {}
    topics_result = lda_model.show_topics(num_topics=best_topic_numer, num_words=5, log=False, formatted=False)
    for topic_id, tokens_probability_for_topic in topics_result:
        word_representation = ", ".join([token for token, _prob in tokens_probability_for_topic])
        print("Topic #{}: {}".format(topic_id, word_representation))
        topic_dict[topic_id] = word_representation

    print(topic_dict)
    return_df = pd.DataFrame.from_dict(topic_dict, orient='index', columns=['UsageInstructionTypeName'])
    return_df.to_csv('resources/lda_topics_usage_instruction_type.csv', index=True)
    return return_df