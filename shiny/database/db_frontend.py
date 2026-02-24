database = "copytaskDB"
port = 5000

import db_manager

from flask import Flask, request
from flask_restful import Resource, Api

app = Flask(__name__)
api = Api(app)

class Uniques(Resource):
    def get(self, table_name, column_name):
        uniques = db_manager.uniques(database, table_name, column_name)
        return {"uniques": uniques}

class Columns(Resource):
    def get(self, table_name):
        columns = db_manager.retrievable_columns(database, table_name)
        return {"columns": columns}

class GetWithColumn(Resource):
    def get(self, table_name, column_name):
        values = db_manager.get(database, table_name, column_name)
        return {column_name: values}

class GetLatest(Resource):
    def get(self, table_name, amount):
        values = db_manager.get_latest(database, table_name, amount)
        return values

class GetRandom(Resource):
    def get(self, table_name, amount):
        values = db_manager.get_random(database, table_name, amount)
        return values

class Count(Resource):
    def get(self):
        count = db_manager.count(database)
        return {"count": count}

class RetrieveById(Resource):
    def post(self, table_name):
        ids = request.json['ids']
        values = db_manager.retrieve_by_id(database, table_name, ids)
        return values

api.add_resource(Uniques, "/uniques/<string:table_name>/<string:column_name>")
api.add_resource(Columns, "/columns/<string:table_name>")
api.add_resource(GetWithColumn, "/get/<string:table_name>/column/<string:column_name>")
api.add_resource(GetLatest, "/get/<string:table_name>/latest/<int:amount>")
api.add_resource(GetRandom, "/get/<string:table_name>/random/<int:amount>")
api.add_resource(RetrieveById, "/get/<string:table_name>/byid")
api.add_resource(Count, "/count")

if __name__ == '__main__':
    app.run(port=port)
