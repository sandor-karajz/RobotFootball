import React, { Component } from 'react';
import { GameStatus } from '../enums';
import { Game } from '../models';
import Character, { CharacterType } from './Character';

interface RobotFootballProps {
    game: Game;
}

export default class RobotFootballManager extends Component<RobotFootballProps> {
    private renderStatus() {
        const { game } = this.props;

        let status: string;

        switch (game.status) {
            case GameStatus.WaitingForPlayer:
                status = 'Waiting for player';
                break;
            case GameStatus.InProgress:
                status = 'In progress';
                break;
            case GameStatus.Finished:
                status = 'Finished';
                break;
            default:
                status = '';
        }

        return (
            <div className="status">
                {status}
            </div>
        );
    }

    private renderScore(score: number, player: boolean) {
        const scoreClass = `score ${player ? '' : 'opponent'}`;

        return (
            <div className="score-container">
                <div className="player">
                    {player ? 'Player' : 'Opponent'}
                </div>
                <div className={scoreClass}>
                    {score}
                </div>
            </div>
        );
    }

    private renderHeader() {
        const { game } = this.props;

        return (
            <div className="header">
                <div className="scores">
                    {game.playerScores.map((x, i) => this.renderScore(x, i === game.playerIndex))}
                </div>
                {this.renderStatus()}
            </div>
        );
    }

    private renderFieldLines() {
        return (
            <>
                <div className="penalty-arc" />
                <div className="penalty-area" />
                <div className="goal" />
                <div className="goal-bar" />
                <div className="penalty-spot" />
                <div className="center-line" />
                <div className="center" />
            </>
        );
    }

    private renderField() {
        const { game } = this.props;

        return (
            <div className="field-container">
                <div className="field-inner-container">
                    <div className="field">
                        {this.renderFieldLines()}
                        {game.players.map(x => <Character object={x} type={x.isOpponent ? CharacterType.Opponent : CharacterType.Player} />)}
                        <Character object={game.ball} type={CharacterType.Ball} />
                    </div>
                </div>
            </div>
        );
    }

    render() {
        const { game } = this.props;
        
        return game && (
            <div className="game-container">
                {this.renderHeader()}
                {this.renderField()}
            </div>
        );
    }
}
