# Networking - Event based networking solution for Unity

A custom networking solution based around a signals architechture from my library [justinleemans/signals](https://github.com/justinleemans/signals).

# Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
    - [Running a server](#running-a-server)
    - [Connecting a client](#connecting-a-client)
    - [Running the update loop](#running-the-update-loop)
    - [Creating messages](#creating-messages)
    - [Sending messages](#sending-messages)
    - [Receiving messages](#receiving-messages)
- [Transports](#transports)
    - [Tcp transport](#tcp-transport)
    - [Creating a custom transport](#creating-a-custom-transport)
- [Contributing](#contributing)

# Installation

Currently the best way to include this package in your project is through the unity package manager. Add the package using the git URL of this repo: https://github.com/justinleemans/networking.git

# Quick Start

> [!NOTE]
> The quick start guide is not finished and will be updated as the project progresses.

This is a very lightweight networking package. As such a lot of the setup is made easy for you. However the actual architecture of your networking implementation is up to you.

This package can be used to create a client-host setup where one of the clients will host the server on their machine from within the game. But this package could also be used to create dedicated client and server applications.

## Running a server

To run a server you simply first have to create a server instance. When creating a server instance you have the option to choose a transport layer by passing a transport layer instance in the constructor. Currently the default transport layer is TCP. For more info on transports take a look at [transports](#transports).

```c#
Server server = new Server();
Server server = new Server(new TcpServerTransport());
```

Once you have your server instance you can start the server. Simply call the method `Start()`. Before starting your server remember to set the correct connection details on your transport. For more info see [transports](#transports).

If you want to stop the server again simply call `Stop()`.

```c#
server.Start();
server.Stop();
```

The server also has two events that can be subscribed to for when a client either connects or disconnects to/from the server.

```c#
server.OnClientConnected += OnClientConnected;
server.OnClientDisconnected += OnClientDisconnected;
```

Both these events take a delegate with an integer as parameter which represents the connection id that has connected/disconnected.

## Connecting a client

To connect a client to a server you will first need a client instance. This is practically the same as for the server.

```c#
Client client = new Client();
Client client = new Client(new TcpClientTransport());
```

Once you have your instance you can start connecting to a server. For this you can call the method `Connect()`. Before connecting the client remember to set the correct connection details on your transport. For more info see [transports](#transports).

If you want to disconnect your client you can call `Disconnect()`.

```c#
client.Connect();
client.Disconnect();
```

## Running the update loop

To make sure your peer is receiving all communications and managing all connections you have to consistently update the peer by calling the `Tick()` method. This goes for both server and client. It is recommended to call this method from the `FixedUpdate()` method on a MonBehaviour or through a similar approach. This is because you don't want you communications to be framerate dependant.

## Creating messages

The message side of this system is heavily based on my signals library and because of that the process is mostly similar except for a few small changes. For more info see [justinleemans/signals](https://github.com/justinleemans/signals).

> [!NOTE]
> In this library we use messages instead of signals. This is mostly a naming difference.

To create a message we make a new class which inherits from the abstract class `Message`, this class is used for all message instances you will be sending and receiving. This class is also where you will define all fields/properties that you want to pass through.

Furthermore all message classes need to include the `Message` attribute above the class with a unique id(int) to identify the message.

```c#
[Message(1)]
public class ExampleMessage : Message
{
}
```

> [!TIP]
> Define all message ids as constants in a class to more easily keep track of what ids are used.

Just like with the signals library you have the option to override the method `OnClear()` which will be called whenever the message class get released back to the message pool. In this library it is extra important to override this method because you don't want to accidentally send values that were set in a different message call.

```c#
public override void OnClear()
{
    Foo = default;
}
```

Besides the `OnClear()` method two other overridable methods have been added. `OnSerialize(IWriteDataStream dataStream)` and `OnDeserialize(IReadDataStream dataStream)`. These methods are used to read/write the values of fields and properties to/from the data stream.

```c#
public override void OnSerialize(IWriteDataStream dataStream)
{
    dataStream.WriteBool(boolVariable);
    dataStream.WriteFloat(floatVariable);
    dataStream.WriteInt(intVariable);
    dataStream.WriteString(stringVariable);
}
```

```c#
public override void OnDeserialize(IReadDataStream dataStream)
{
    boolVariable = dataStream.ReadBool();
    floatVariable = dataStream.ReadFloat();
    intVariable = dataStream.ReadInt();
    stringVariable = dataStream.ReadString();
}
```

## Sending messages

To send a message over the network you can either get a message and populate any fields or properties you have on your message class by getting a message with `GetMessage<TMessage>()` and sending it with `SendMessage(message)`.

These methods are available on either your server or client instance depending on which side is sending the message. Keep in mind that when a client sends a message it is the server that will receive it and if server is the one sending a message, all connected clients will receive it.

```c#
var message = client.GetMessage<ExampleMessage>();
message.Foo = "bar";
client.SendMessage(message);
```

Or send it directly with `SendMessage<TMessage>()` without populating fields or properties.

```c#
client.SendMessage<TMessage>();
```

> [!WARNING]
> It is recommended to use the included `GetMessage<TMessage>()` or `SendMessage<TMessage>()` methods to retrieve a message instance to avoid filling up the pool and never retrieving from it.

In the case of the server you get an extra set of methods so you can send to specific connections. These methods are the same as the other methods but with an extra parameter.

```c#
server.SendMessage<TMessage>(connectionId);
server.SendMessage(message, connectionId);
```

## Receiving messages

You can subscribe to a message using either the server or client instance and calling `Subscribe<TMessage>(OnMessage)` where the handler is a delegate with a message instance of the given message type as parameter.

```c#
client.Subscribe<ExampleMessage>(OnMessage);
```

```c#
private void OnExampleMessage(ExampleMessage message)
{
}
```

To unsubscribe from a message you make a call similar to subscribing by calling `Unsubscribe<TMessage>(OnMessage)` with the same method as you used to subscribe earlier.

```c#
client.Unsubscribe<ExampleMessage>(OnMessage);
```

Same as with sending message, the server class has a set of extra methods that allow you to see which connection has sent a message. This simply changes the delegate to include a connection id.

```c#
private void OnExampleMessage(ExampleMessage message, int connectionId)
{
}
```

# Transports

The currently implemented and available transports are:
- [Tcp transport](#tcp-transport) (default)

## Tcp transport

The tcp transport uses a tcp protocol for connecting and sending data across a network.

These connection details are part of the transport and can be set through properties when initializing or before starting/connection the peer.

```c#
IServerTransport transport = new TcpServerTransport()
{
    Port = 7777,
    MaxConnections = 10,
}

transport.Port = 7777;
transport.MaxConnections = 10;
```

```c#
IClientTransport transport = new TcpClientTransport()
{
    IpAddress = "127.0.0.1",
    Port = 7777,
}

transport.IpAddress = "127.0.0.1";
transport.Port = 7777;
```

## Creating a custom transport

If you want to implement your own transport there is a few things you will have to do. You can take a look at the included [Tcp transport](https://github.com/justinleemans/networking/tree/main/Runtime/Transports/Tcp) as an example.

You will have to create a class implementing the `IServerTransport` interface for the server implementation. This will require you to implement 4 events, properties or methods.
- `Action<Connection> OnNewConnection` which is an event that should be called when a new connection is made. Should return this new connection.
- `void Start()` which is to start the server.
- `void Stop()` which is to stop the server.
- `void Tick()` this method is the update loop for your transport, you will use this for checking wether you are able to receive a message.

Next you will have to create a class implementing the `IClientTransport` interface. This will also require you to implement 3 methods.
- `Connection Connect(string remoteAddress, ushort port)` which is used to connect this client to a server. Should return an instance of Connection or null if failed to connect.
- `void Disconnect()` which is to disconnect this client from the server.
- `void Tick()` this method is the update loop for your transport, you will use this for checking wether you are able to receive a message.

And lastly you will have to create a class deriving from `Connection`. This is the connection representing your peer.

Next there is 3 methods you will have to implement.
- `OnSend(byte[] dataBuffer)` which is used for sending the data. You get a byte array which you will have to send through your method of choice. Further manipulation of this byte array is generally not needed.
- `OnReceive(out byte[] dataBuffer)` which is used to check if you have data that you can read. You will have to set the `dataBuffer` variable before exiting the method. If you don't have enough bytes for a full message or the data is not ready to be send through you can leave this at `null`. The `dataBuffer` will always come prefixed with a length int and a message id int. You have to read the length data and remove it before passing it on. You need to leave the message id in there.
- `OnClose()` which will execute the code to close this connection.

# Contributing

Currently I have no set way for people to contribute to this project. If you have any suggestions regarding improving on this project you can make a ticket on the GitHub repository or contact me directly.
