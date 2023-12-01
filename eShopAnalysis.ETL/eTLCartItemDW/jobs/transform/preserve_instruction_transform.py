import pandas as pd
from pandas import DataFrame
import spacy
import re
from spacy.tokens import Token

#refer to nlp.py for more info and easy run and viewing result at each step
from jobs.utils.nlp_utils import map_to_processed_tokens
nlp = spacy.load('en_core_web_md')

def transform_to_preserve_instruction_df_with_topic_and_word_representation(preserve_instruction_df: DataFrame, best_topic_numer: int = 8) -> DataFrame:
    """
    Convert preserve instruction dataframe to bag of words dataframe
    Then train LDA model on the bag of words dataframe to get topic and word representation for each topic
    Return dataframe with columns: TopicId, TopicWords as result"""
    processed_tokens_lists = []
    for preserve_instruction_text in preserve_instruction_df['PreserveInstructionTypeName']:
        processed_tokens_lists.append(map_to_processed_tokens(preserve_instruction_text))
        print(processed_tokens_lists[-1])

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
    lda_model.save("resources/lda_preserve_instruction.model")
    topic_dict = {
        -1: "No topic or cannot be classified",
    }
    topics_result = lda_model.show_topics(num_topics=best_topic_numer, num_words=5, log=False, formatted=False)
    for topic_id, tokens_probability_for_topic in topics_result:
        word_representation = ", ".join([token for token, _prob in tokens_probability_for_topic])
        print("Topic #{}: {}".format(topic_id, word_representation))
        topic_dict[topic_id] = word_representation

    print(topic_dict)
    return_df = pd.DataFrame.from_dict(topic_dict, orient='index', columns=['PreserveInstructionTypeName'])
    return_df.to_csv('resources/lda_topics_preserve_instruction_type.csv', index=True)
    return return_df