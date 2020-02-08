using System;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperWebSocket;


namespace SpeechRecognitionAPI
{
    class SuperSocketTest
    {
        SuperSocketTest()
        {
            // コンフィグオブジェクト作成
            var rootConfig = new SuperSocket.SocketBase.Config.RootConfig();
            var serverConfig = new SuperSocket.SocketBase.Config.ServerConfig()
            {
                Port = 2012,
                Ip = "Any",
                MaxConnectionNumber = 100,
                Mode = SuperSocket.SocketBase.SocketMode.Tcp,
                Name = "SuperWebSocket Sample Server"
            };

            //サーバーオブジェクト作成＆初期化
            var server = new SuperWebSocket.WebSocketServer();
            //server.Setup(rootConfig, serverConfig, SuperSocket.SocketEngine.SocketServerFactory.Instance);

            /*
            //イベントハンドラの設定
            //接続
            server.NewSessionConnected += HandleServerNewSessionConnected;
            //メッセージ受信
            server.NewMessageReceived += HandleServerNewMessageReceived;
            //切断        
            server.SessionClosed += HandleServerSessionClosed;
            */

            //サーバー起動
            Console.WriteLine("Start!");
            server.Start();
        }
    }
}
