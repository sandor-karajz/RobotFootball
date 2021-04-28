import React, { Component, CSSProperties } from 'react';
import { GameObject } from '../models';
import '../css/character.css';
import { GOAL_LINE_LENGHT, TOUCHLINE_LENGHT } from '../constants';

export enum CharacterType {
    Ball,
    Player,
    Opponent
}

interface CharacterProps {
    object: GameObject;
    type: CharacterType;
     
}

export default class Character extends Component<CharacterProps> {
    private getClass() {
        const { type } = this.props;

        switch (type) {
            case CharacterType.Ball:
                return 'ball';
            case CharacterType.Player:
                return 'player';
            case CharacterType.Opponent:
                return 'opponent';
        }
    }

    private addPercentageSign = (value: number) => `${value}%`;

    render() {
        const { object } = this.props;

        const playerClass = `character ${this.getClass()}`;

        const widthPercentage = (object.radius * 2) / TOUCHLINE_LENGHT * 100;
        const heightPercentage = (object.radius * 2) / GOAL_LINE_LENGHT * 100;
        const leftPercentage = (object.position.x - object.radius) / TOUCHLINE_LENGHT * 100;
        const bottomPercentage = (object.position.y - object.radius) / GOAL_LINE_LENGHT * 100;

        const style: CSSProperties = {
            width: this.addPercentageSign(widthPercentage),
            height: this.addPercentageSign(heightPercentage),
            left: this.addPercentageSign(leftPercentage),
            bottom: this.addPercentageSign(bottomPercentage)
        };

        return <div className={playerClass} style={style} />;
    }
}
