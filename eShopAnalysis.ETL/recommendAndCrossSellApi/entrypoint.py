from flask import Flask
from flask_cors import CORS
from routes.blueprint import blueprint

def create_app():
    app = Flask(__name__)
    CORS(
        app, 
        origins="http://localhost:4200", 
        methods=["GET", "POST", "PUT", "DELETE"], 
    )
    return app

app = create_app()
app.register_blueprint(blueprint)

if __name__ == '__main__':
    app.run(port=5000)