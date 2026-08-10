# Use Flask & Flask-Restful to manipulate the database.

# Endpoints : uniques/{table-name}/{column-name} => Get a iist of all unique values of a single column of a single table
#             column/{table-name}/{column-name}  => Get all values of a column of a table
#             filter  => Get all with a filter, TODO: think more about this one!

import psycopg2
import psycopg2.extras
from psycopg2 import sql
import os
import io
import math

username = "inputlog"
password = "X(@#5_/U9kpa5?]a_8"
port = 5432

# Convert a list of 1-item tuples to a list of items.
def to_list(list_of_tuples):
    return [item[0] for item in list_of_tuples if item[0] != None]

# Fetch all values in the form of a dictionary : i.e. if we have 2 columns and 3 values per column, we have a dictionary with 2 keys and each value is a list with 3 values.
def fetch_dictionary(cursor):
    values = {}

    value = cursor.fetchone()
    while value:
        for key in value.keys():
            if key in values:
                values[key].append(value[key])
            else:
                values[key] = [value[key]]
        value = cursor.fetchone()

    return values

# Connect with the default database and create a new database.
def create_database(db_name):
    conn = psycopg2.connect(dbname="inputlog", user=username, password=password, port=port)
    conn.set_isolation_level(psycopg2.extensions.ISOLATION_LEVEL_AUTOCOMMIT)
    cursor = conn.cursor()
    cursor.execute(sql.SQL("CREATE DATABASE {}").format(sql.Identifier(db_name)))
    
# Setup the structure of all the tables in the database.
def initial_setup(db_name:str):
    with psycopg2.connect(dbname=inputlog, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()

        cursor.execute("""CREATE TABLE session(
        id SERIAL PRIMARY KEY,
        uuid  CHAR(16),
        language CHAR(2),
        age INT,
        gender VARCHAR(10),
        session TEXT,
        keyboard CHAR(6),
        test_group TEXT,
        experience VARCHAR(50),
        handedness REAL,
        computer VARCHAR(10),
        familiarity INT,  
        browser VARCHAR(20),
        disorder BOOLEAN,
        education TEXT,
        repetition BOOLEAN,
        correctness REAL,
        cpm REAL,
        score REAL GENERATED ALWAYS AS (LEAST(correctness * cpm/100, 600)) STORED, -- calculate the score as a percentage of the ratio of cpm vs max cpm (600)
        creation_time TIMESTAMP
        );""")

        cursor.execute("""CREATE TABLE components_tapping(
        id int REFERENCES session (id),
        targetted int,
        non_targetted int,
        cpm float,
        median float,
        stdev float,
        logmean float,
        mean_iki float
        );""")

        table_names = []
        for a in ["words1", "words2", "words3", "words4"]:
            for b in range(1,8):
                table_names.append("trials_"+a+"_trial"+str(b))
            table_names.append("trials_"+a+"_overall")

        table_names += ["components_consonants", "components_sentence"]

        table_names += ["frequency_low", "frequency_high"]

        table_names += ["hands_rl", "hands_lr", "hands_rr", "hands_ll"]

        table_names += ["adjacency_true", "adjacency_false"]

        table_names += ["repetition_true", "repetition_false"]

        for table_name in table_names:
            query = sql.SQL("CREATE TABLE {0} (like components_tapping including all);").format(sql.Identifier(table_name))
            cursor.execute(query)

        for i in range(1,5):
            query = sql.SQL("CREATE VIEW {0} AS SELECT * FROM {1}").format(sql.Identifier("components_words"+str(i)), sql.Identifier("trials_words"+str(i)+"_overall"))
            cursor.execute(query)

        # Creates a trigger that will determine for each row at insertion if the experiment is a repetition or not.
        #
        # ! We use AFTER, for the case of batch inserts, where repetitions might occur in the same batch
        # ! that wouldn't be recognised by the first statement if we use BEFORE as they then aren't in the database yet.
        #
        # The trigger works by selecting the first ID with matching (uuidXlanguage) pair from the database,
        # comparing the ID with the ID of our inserted row and see if they match.
        # If matching, our row is the chronological first and is not a repetition, if not then it is a repetition.
        #
        # If we want to sort by creation date, it is possible that an earlier experiment will have a higher ID, as such then the repeated experiments with a later creation_time need also be updated.
        cursor.execute("""CREATE OR REPLACE FUNCTION calculate_repetition()
                            RETURNS trigger
                            LANGUAGE plpgsql
                            VOLATILE
                            AS $function$
                            DECLARE 
                            original_id        integer;
                            record             session%rowtype;
                            BEGIN

                            EXECUTE format('SELECT id FROM %I WHERE uuid = $1.uuid AND language = $1.language ORDER BY creation_time LIMIT 1 ', TG_TABLE_NAME)
                            INTO original_id
                            USING NEW;

                            EXECUTE format('UPDATE %I set repetition = %L WHERE id = $1.id', TG_TABLE_NAME, NEW.id <> original_id)
                            USING NEW;

                            FOR record IN
                                EXECUTE format('SELECT * FROM %I WHERE uuid = $1.uuid AND language = $1.language AND repetition = FALSE and creation_time > $1.creation_time', TG_TABLE_NAME) 
                                USING NEW
                            LOOP

                                EXECUTE format('UPDATE %I set repetition = TRUE WHERE id = $1.id', TG_TABLE_NAME)
                                USING record;
                            
                            END LOOP;

                            RETURN NEW;
                            END;
                            $function$;

                            CREATE TRIGGER repetition_trigger AFTER INSERT ON session FOR EACH ROW EXECUTE PROCEDURE calculate_repetition();""")


# Get all unique values of a column in a table.
def uniques(db_name:str, table_name:str, column_name:str):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()
        query = sql.SQL("SELECT DISTINCT {0} from {1};").format(sql.Identifier(column_name), sql.Identifier(table_name))
        cursor.execute(query)

        values = cursor.fetchall()
        return to_list(values)

# Get a list of all columns in a table.
def column_names(db_name:str, table_name:str):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = %s ORDER BY ordinal_position", (table_name,))
        values = [value for value in to_list(cursor.fetchall())]
        return values

# Generte a list of which columns can be retrieved.
def retrievable_columns(db_name:str, table_name:str):
    colnames = column_names(db_name, table_name)
    
    remove = ['id', 'uuid', 'creation_time']
    
    if (table_name == 'session'):
        colnames = [col for col in colnames if col not in remove]
        
    return colnames

# Get a table from a database.
def get(db_name:str, table_name:str):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()
        query = sql.SQL("SELECT * FROM {0};").format(sql.Identifier(table_name))
        cursor.execute(query)
        values = cursor.fetchall()
        return values

# This is typically rather slow with very large databases, but the assumption will be that there are at most 10.000 - 100.000 samples in the database.
def count(db_name:str):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()
        query = sql.SQL("SELECT COUNT(*) FROM session")
        cursor.execute(query)
        return cursor.fetchone()[0]

# Get a column from a table in a database.
def get(db_name:str, table_name:str, column_name:str):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()
        query = sql.SQL("SELECT {0} FROM {1};").format(sql.Identifier(column_name), sql.Identifier(table_name))
        cursor.execute(query)
        values = to_list(cursor.fetchall())
        return values

# Get the latest entries from a table (determined by id).
def get_latest(db_name:str, table_name:str, amount:int):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor(cursor_factory=psycopg2.extras.DictCursor)
        query = sql.SQL("SELECT * FROM {0} ORDER BY id DESC LIMIT {1};").format(sql.Identifier(table_name), sql.SQL(str(amount)))
        cursor.execute(query)
        values = fetch_dictionary(cursor)
        return values

# !! This is typically rather slow with very large databases, but the assumption will be that there are at most 10.000 - 100.000 samples in the database.
# !! Bernoulli sampling doesn't garantuee a fixed amount of results, instead each entry has a fixed percentage to be sampled.
# !! To circumvent this issue, we keep sampling intelligently until the desired amount.
def get_random(db_name:str, table_name:str, amount:int):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        values = {"id":[]}
        columns = sql.SQL(",").join(sql.Identifier(col) for col in ["id"] + retrievable_columns(db_name, table_name))
        while len(values["id"]) < amount:  
            cursor = conn.cursor(cursor_factory=psycopg2.extras.DictCursor)
            count_ = count(db_name)

            percentage = 100*amount/count_ 

            query = sql.SQL("SELECT {0} FROM {1} TABLESAMPLE BERNOULLI({2}) LIMIT {3};").format(columns, sql.Identifier(table_name), sql.SQL(str(percentage)), sql.SQL(str(amount)))
            cursor.execute(query)
            new_values = fetch_dictionary(cursor)
            if len(values["id"]) > 0:
                indices = [i for i in range(len(new_values["id"])) if new_values["id"][i] not in values["id"]]
                for key, vals in values.items():
                    values[key] = values[key] + [new_values[key][i] for i in indices]
                    if len(values[key]) > amount:
                        values[key] = values[key][:amount]
                print(len(values["id"]))
            else:
                values = new_values
        return values

# Retrieve a set of records from a table in a database where the id occurs in the given list of ids.
def retrieve_by_id(db_name:str, table_name:str, ids:[int]):
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor(cursor_factory=psycopg2.extras.DictCursor)

        query = sql.SQL("SELECT * FROM {0} WHERE id in %s;").format(sql.Identifier(table_name))
        cursor.execute(query, (tuple(ids),))
        values = fetch_dictionary(cursor)
        if 'creation_time' in values.keys():  # TODO: this is a temporary fix as datetime objects are not JSON serialisable. The question is do we need the datetime in the app other than for calculating repetition?
            values.pop('creation_time')
        return values

# Populate a table in a database from a csv file.
def copy_csv(filename, table_name, db_name:str="test2", start_line=0):
    stream = open(filename, "r")
    if start_line != 0: # Such that we only insert new lines, skip all lines before the start line (the first new line)
        for i, line in enumerate(stream):
            if i == start_line - 1:
                break
    colnames = column_names(db_name, table_name)
    if table_name == "session":  # We cannot copy auto-generated columns.
        colnames.remove('id')
        colnames.remove('score')
    columns = sql.SQL(", ").join([sql.Identifier(col) for col in colnames])
    with psycopg2.connect(dbname=db_name, user=username, password=password, port=port) as conn:
        cursor = conn.cursor()
        cursor.copy_expert(sql.SQL("COPY {0}({1}) FROM STDIN CSV HEADER").format(sql.Identifier(table_name), columns), stream)

# Populate a database from a set of csv files.
def populate_from_files(root, db_name:str="test2"):    
    for root2, dirs, fnames in os.walk(root):
        for fname in fnames:
            name = os.path.relpath(os.path.join(root2,fname), root)
            if os.path.basename(name) in ["words1.csv", "words2.csv", "words3.csv", "words4.csv"]:
                continue
            print(os.path.join(root2, fname))
            name = name.replace("/", "_")
            name = name.replace("\\", "_")
            name = name.split(".csv")[0]
            copy_csv(os.path.join(root2, fname), name, db_name)
