from flask import Blueprint
import controllers.cross_sell_controller as cross_sell_controller
import controllers.recommend_controller as recommend_controller
blueprint = Blueprint("recommendAndCrossSellApi", __name__)

blueprint.route("/cross_sell", methods=["POST"])(cross_sell_controller.cross_sell)
blueprint.route("/recommend", methods=["GET"])(recommend_controller.recommend)