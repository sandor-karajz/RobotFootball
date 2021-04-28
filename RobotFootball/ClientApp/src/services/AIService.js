"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Utilities_1 = require("../utilities/Utilities");
var enums_1 = require("../enums");
var constants_1 = require("../constants");
var AIService = /** @class */ (function () {
    function AIService() {
    }
    AIService.getCommands = function (game) {
        var _this = this;
        if (game.status !== enums_1.GameStatus.InProgress) {
            return [];
        }
        var ballPosition = game.ball.position;
        var shouldFlip = game.playerIndex === 1;
        var areaRatio = this.getBallAreaRatio(ballPosition, shouldFlip);
        var playerPositions = this.getPlayerPositions(areaRatio);
        var ownPlayers = game.players.filter(function (x) { return !x.isOpponent; });
        var closestPlayer = this.getClosestPlayerToBall(ownPlayers, ballPosition);
        return ownPlayers.map(function (x) {
            var isClosest = _this.isClosestPlayer(x, closestPlayer);
            return {
                playerId: x.playerId,
                targetX: isClosest ? ballPosition.x : _this.flip(playerPositions[x.playerId].x, constants_1.TOUCHLINE_LENGHT, shouldFlip),
                targetY: isClosest ? ballPosition.y : _this.flip(playerPositions[x.playerId].y, constants_1.GOAL_LINE_LENGHT, shouldFlip),
                isShooting: isClosest
            };
        });
    };
    AIService.getBallAreaRatio = function (ballPosition, shouldFlip) {
        return {
            x: (shouldFlip ? constants_1.TOUCHLINE_LENGHT - ballPosition.x : ballPosition.x) / constants_1.TOUCHLINE_LENGHT,
            y: ballPosition.y / constants_1.GOAL_LINE_LENGHT
        };
    };
    AIService.getPlayerPositions = function (areaRatio) {
        var _this = this;
        return constants_1.AREAS.map(function (x) { return ({
            x: _this.getPositionFromArea(x.bottomLeft.x, x.topRight.x, areaRatio.x),
            y: _this.getPositionFromArea(x.bottomLeft.y, x.topRight.y, areaRatio.x)
        }); });
    };
    AIService.getClosestPlayerToBall = function (players, ballPosition) {
        return players.sort(function (a, b) { return Utilities_1.default.getDistance(a.position, ballPosition) - Utilities_1.default.getDistance(b.position, ballPosition); })[0];
    };
    AIService.isClosestPlayer = function (player, closestPlayer) {
        return player.playerId === closestPlayer.playerId;
    };
    AIService.flip = function (value, max, shouldFlip) {
        return shouldFlip ? max - value : value;
    };
    AIService.getPositionFromArea = function (bottomLeft, topRight, ratio) {
        return bottomLeft + (topRight - bottomLeft) * ratio;
    };
    return AIService;
}());
exports.default = AIService;
//# sourceMappingURL=AIService.js.map