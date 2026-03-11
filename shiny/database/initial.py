from db_manager import *

db_name = input("Enter a name for the database: ")
create_database(db_name)

print("Setting up the database")
initial_setup(db_name)

corpus_root = input("Give the root adress of the corpus (the folder containing 'bigrams', 'data', 'idfx', 'xml'): ")
corpus_root = corpus_root + "/data"
print("Populating...")
populate_from_files(corpus_root, db_name)
