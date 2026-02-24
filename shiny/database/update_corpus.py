from db_manager import *

corpus_root = input("Give the root adress of the corpus (the folder containing 'bigrams', 'data', 'idfx', 'xml'): ")
corpus_root = corpus_root + "/data"
print("Populating...")
populate_from_files(corpus_root, "copytaskDB")