import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import {Board} from '../../models/board';
import {BoardCommunicationService} from '../communication/board-communication.service';
import {Card} from '../../models/card';
import {CardLabelCommunicationService} from '../communication/card-label-communication.service';
import {CardCommunicationService} from '../communication/card-communication.service';
import {ListCommunicationService} from '../communication/list-communication.service';
import {LabelCommunicationService} from '../communication/label-communication.service';
import {UserBoardCommunicationService} from '../communication/user-board-communication.service';
import {UserCardCommunicationService} from '../communication/user-card-communication.service';
import {CommentCommunicationService} from '../communication/comment-communication.service';
import {List} from '../../models/list';
import {Label} from '../../models/label';
import {Comment} from '../../models/comment';
import {CardLabel} from '../../models/card-label';
import {UserCard} from '../../models/user-card';
import {UserBoard} from '../../models/user-board';

@Injectable({
  providedIn: 'root'
})
export class BoardHubService {
  private hubConnection!: signalR.HubConnection;

  constructor(private boardCommunicationService: BoardCommunicationService,
              private cardCommunicationService: CardCommunicationService,
              private listCommunicationService: ListCommunicationService,
              private labelCommunicationService: LabelCommunicationService,
              private cardLabelCommunicationService: CardLabelCommunicationService,
              private userCardCommunicationService: UserCardCommunicationService,
              private userBoardCommunicationService: UserBoardCommunicationService,
              private commentCommunicationService: CommentCommunicationService,) {
  }

  connectToBoard(boardId: number): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://trellobackend-d7d3h2dxhwecf7az.brazilsouth-01.azurewebsites.net/boardHub")
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log("conecta", boardId)
        this.joinBoardGroup(boardId);
      })
      .catch((err) => console.error("Error en la conexión", err));

    this.addBoardListeners();
    this.addCardListeners();
    this.addCommentListeners();
    this.addListListeners();
    this.addLabelListeners();
    this.addCardLabelListeners();
    this.addUserBoardListeners();
    this.addUserCardListeners();
  }

  private joinBoardGroup(boardId: number): void {
    this.hubConnection.invoke("JoinBoardGroup", boardId.toString()).catch(err => console.error(err));
  }

  disconnectFromBoard(boardId: number): void {
    this.hubConnection.invoke("LeaveBoardGroup", boardId.toString()).catch(err => console.error(err));
    this.hubConnection.stop();
  }
  // .withUrl("http://localhost:5182/boardHub")

  private addBoardListeners(): void {
    this.hubConnection.on("BoardUpdated", (board: Board) => {
      this.boardCommunicationService.setUpdateBoard(board);
    });

    this.hubConnection.on("BoardDeleted", (boardId: number) => {
      this.boardCommunicationService.setDeleteBoard(boardId);
    });
  }

  private addCardListeners(): void {
    this.hubConnection.on("CardCreated", (card: Card) => {
      this.cardCommunicationService.setAddCard(card);
    });

    this.hubConnection.on("CardUpdated", (card: Card) => {
      this.cardCommunicationService.setUpdateCard(card);
    });

    this.hubConnection.on("CardDeleted", (cardId: number) => {
      this.cardCommunicationService.setDeleteCard(cardId);
    });
  }

  private addCommentListeners(): void {
    this.hubConnection.on("CommentCreated", (comment: Comment) => {
      this.commentCommunicationService.setAddComment(comment);
    });

    this.hubConnection.on("CommentUpdated", (comment: Comment) => {
      this.commentCommunicationService.setUpdateComment(comment);
    });

    this.hubConnection.on("CommentDeleted", (commentId: number) => {
      this.commentCommunicationService.setDeleteComment(commentId);
    });
  }

  private addListListeners(): void {
    this.hubConnection.on("ListCreated", (list: List) => {
      console.log(list)
      this.listCommunicationService.setAddList(list);
    });

    this.hubConnection.on("ListUpdated", (list: List) => {
      this.listCommunicationService.setUpdateList(list);
    });

    this.hubConnection.on("ListDeleted", (listId: number) => {
      this.listCommunicationService.setDeleteList(listId);
    });
  }

  private addLabelListeners(): void {
    this.hubConnection.on("LabelCreated", (label: Label) => {
      this.labelCommunicationService.setAddLabel(label);
    });

    this.hubConnection.on("LabelUpdated", (label: Label) => {
      this.labelCommunicationService.setUpdateLabel(label);
    });

    this.hubConnection.on("LabelDeleted", (labelId: number) => {
      this.labelCommunicationService.setDeleteLabel(labelId);
    });
  }

  private addCardLabelListeners(): void {
    this.hubConnection.on("CardLabelCreated", (cardLabel: CardLabel) => {
      this.cardLabelCommunicationService.setAddCardLabel(cardLabel);
    });

    this.hubConnection.on("CardLabelDeleted", (cardId: number, labelId: number) => {
      this.cardLabelCommunicationService.setDeleteCardLabel(cardId, labelId);
    });
  }

  private addUserCardListeners(): void {
    this.hubConnection.on("UserCardCreated", (userCard: UserCard) => {
      this.userCardCommunicationService.setAddUserCard(userCard);
    });

    this.hubConnection.on("UserCardDeleted", (userId: number, cardId: number) => {
      this.userCardCommunicationService.setDeleteUserCard(userId, cardId);
    });
  }

  private addUserBoardListeners(): void {
    this.hubConnection.on("UserBoardCreated", (userBoard: UserBoard) => {
      this.userBoardCommunicationService.setAddUserBoard(userBoard);
    });

    this.hubConnection.on("UserBoardDeleted", (boardId: number, userId: number): void => {
      this.userBoardCommunicationService.setDeleteUserBoard(userId, boardId);
    });
  }
}
