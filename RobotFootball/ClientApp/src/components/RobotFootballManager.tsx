import React, { Component } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel  } from '@aspnet/signalr'
import AIService from '../services/AIService';
import Utilities from '../utilities/Utilities';
import { Command, Game } from '../models';
import RobotFootball from './RobotFootball';
import '../css/robot-football.css';

interface RobotFootballState {
    game: Game;
    hubConnection: HubConnection;
}

export default class RobotFootballManager extends Component<{}, RobotFootballState> {
    private readonly setStateAsync = Utilities.getSetStateAsync(this);

    constructor(props: {}) {
        super(props);

        this.state = {
            game: null,
            hubConnection: null
        };
    }

    async componentDidMount() {
        const hubConnection = new HubConnectionBuilder().withUrl("/robot-football").configureLogging(LogLevel.Information).build();

        hubConnection.on('sendState', this.handleSendState);
        hubConnection.on('disconnect', this.handleDisconnect);

        await this.setStateAsync({ hubConnection });

        await hubConnection.start();
    }
    private handleSendState = async (game: Game) => {
        this.setState({ game });

        const commands = AIService.getCommands(game);

        if (commands.length) {
            this.handleCommand(commands);
        }
    };

    private handleDisconnect = () => this.state.hubConnection.stop();

    private handleCommand = (commands: Command[]) => this.state.hubConnection.invoke('command', commands);

    render() {
        const { game } = this.state;

        return <RobotFootball game={game} />;
    }
}
