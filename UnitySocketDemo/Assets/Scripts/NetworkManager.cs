using UnityEngine;
using UnityEngine.UI;
using SocketCommon;
using System;
using System.Collections;
using System.Net;

public class NetworkManager : MonoBehaviour
{
	public Text text;

	private SocketClientManager client;
	private Logger logger;

	private string textToDisplay;

	public void Start()
	{
		IPAddress serverIp = IPAddress.Parse("192.168.140.122");

		logger = new Logger();
		client = new SocketClientManager(logger, OnReceive, serverIp);

		client.ConnectToServer();
		textToDisplay = "Connected";
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
			PingServer();

		text.text = textToDisplay;
	}

	public void OnDestroy()
	{
		client.Disconnect();
	}

	public void PingServer()
	{
		byte[] toSend = BitConverter.GetBytes(DateTime.Now.Ticks);
		
		//byte[] toSend = new byte[2];
		//toSend[0] = (byte)1;
		//toSend[1] = (byte)2;
		
		client.Send(toSend);
	}

	private void OnReceive(byte[] bytesReceived)
	{
		long sendTicks = BitConverter.ToInt64(bytesReceived, 0);
		long diff = DateTime.Now.Ticks - sendTicks;
		textToDisplay = diff.ToString();
	}
}
