from spacy.tokens import Token
import re
#Used in transform_usage_instruction 
filter_pos = lambda token: token.pos_ != "AUX" and token.pos_ != "DET" and token.pos_ != "PRON"\
                           and token.pos_ != "SCONJ" and token.pos_ != "PUNCT" and token.pos_ != "SYM" and token.pos_ != "X" and token.pos_ != "SPACE"

filter_some_nouns = lambda token: token.lemma_ != "product" and token.lemma_ != "warning" and token.text != "Warning" and token.text != "Warnings"

filter_stop_words = lambda token: not token.is_stop

def map_list_tokens_to_sublists_tokens(list: list[Token]) -> list[list[Token]]:
    """
    Convert list of tokens to list of sublists of tokens, 
    where each sublist is a clause that is separated by conjunctions 
    (and, or, but, etc)
    """
    sublists = []
    sublist = []
    for idx, token in enumerate(list):
        if idx == len(list) - 1:
            sublist.append(token)
            sublists.append(sublist)
            break
        if (token.pos_ == "CCONJ" or (token.pos_ == "PART" and token.text != "to")) and (list[idx+1].pos_ == "VERB" or list[idx+1].dep_ == "xcomp" or list[idx+1].dep_ == "acomp"):
            sublists.append(sublist)
            sublist = []
            if token.text == "not":
                sublist.append(token)
        else:
            sublist.append(token)
    return sublists

def map_to_one_string(list: list[str]) -> str:
    """Join multiple string in list to one string with space as separator"""
    return ' '.join([text_token for text_token in list])

def filter_simple_structure_sublists(sublists: list[list[Token]]) -> list[list[Token]]:
    """Remove simple structure using regex we defined"""
    filtered_sublists = []
    # TODO fine tune these regex
    raw_regex__dep_list_to_keep = ["^neg ROOT.*", "^amod ROOT$", "^advmod ROOT$", "^ROOT"]
    # not (main verb), direct use, better hot
    raw_regex__pos_list_to_keep = ["^VERB.*", "^ADV VERB.*", "^ADV PART VERB.*" , "^PART VERB.*"] 
    # directly to use , served best chilled, served best chilled or over ice, not use ...
    regex__pos_list_to_keep = list(map(lambda regex:re.compile(regex), raw_regex__pos_list_to_keep))
    regex__dep_list_to_keep = list(map(lambda regex:re.compile(regex), raw_regex__dep_list_to_keep))
    for sublist in sublists:
        grammar_tokens_dep = list(map(lambda token: token.dep_, sublist))
        grammar_tokens_pos = list(map(lambda token: token.pos_, sublist))
        dep_str = map_to_one_string(grammar_tokens_dep).strip() # "ROOT advmod"
        pos_str = map_to_one_string(grammar_tokens_pos).strip() # "VERB ADV"
        if (any(regex.match(dep_str) for regex in regex__dep_list_to_keep)\
            or any(regex.match(pos_str) for regex in regex__pos_list_to_keep))\
            and not re.findall(r'.*VERB.*', pos_str).count(".*VERB.*") > 1: #TODO fix later, this exclude multiple verb, except for the one in the list
                filtered_sublists.append(sublist)
    return filtered_sublists

from nltk.stem.porter import PorterStemmer
from nltk.corpus import wordnet
from nltk.stem.wordnet import WordNetLemmatizer
def get_wordnet_pos_from_spacy_pos(spacy_pos: str) -> str:
    """Convert spacy pos tag to wordnet pos tag"""
    if spacy_pos == 'ADJ':
        return wordnet.ADV
    elif spacy_pos == 'VERB':
        return wordnet.VERB
    elif spacy_pos == 'NOUN':
        return wordnet.NOUN
    elif spacy_pos == 'ADV':
        return wordnet.ADV
    else:
        return None # for easy if-statement 
    
def map_to_lemmatized_nltk(token: Token) -> str:
    """nltk lemmatizer can specify pos tag to improve accuracy, but spacy lemmatizer cannot"""
    lemmatizer = WordNetLemmatizer()
    wordnet_pos = get_wordnet_pos_from_spacy_pos(token.pos_)
    print("Wordnet pos: ", wordnet_pos, '\n')
    if wordnet_pos is not None:
        return lemmatizer.lemmatize(token.text, get_wordnet_pos_from_spacy_pos(token.pos_)) #only one text is passed in so the list only have one element
    else:
        return token.text

def map_token_to_stemmed_nltk(token_text: str) -> str:
    """spacy do not have stemmer"""
    porter_stemmer = PorterStemmer()
    return porter_stemmer.stem(token_text)

map_to_lemmatized_spacy = lambda token: token.lemma_.lower()