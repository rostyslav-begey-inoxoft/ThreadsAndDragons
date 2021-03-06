﻿using System;
using System.Net;
using System.Text;
using System.Threading;
using ThreadsAndDragons.Keepers;

namespace ThreadsAndDragons
{
	class Server
	{
		public Server(int port, string[] sentences)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/", port));

			keeper=new Keeper(sentences);
		}

		public void Start()
		{
			Console.WriteLine("Simple listeneing.");
			listener.Start();
			while (true)
			{
				try
				{
					var context = listener.GetContext();
					ReplaceResponse(context);
					context.Response.Close();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		public Tuple<int, string> ReplaceFirst(string word, string replace)
		{
			return keeper.ReplaceFirst(word, replace);
		}

		public void ReplaceResponse(HttpListenerContext context)
		{
			if (context.Request.Url.Query.Length > 0)
			{
				var word = context.Request.QueryString["word"];
				var replace = context.Request.QueryString["replace"];
				context.Request.InputStream.Close();

				Thread.Sleep(1000);

				var res = ReplaceFirst(word, replace);

				var encryptedBytes = Encoding.UTF8.GetBytes(
					string.Format("Change in sentence #{0}\n{1}", res.Item1, res.Item2));

				context.Response.OutputStream.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
				context.Response.OutputStream.Close();
			}
		}

		private HttpListener listener;

		private Keeper keeper;
	}
}
