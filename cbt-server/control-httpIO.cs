using System;
using System.IO;
using cbt_server;
using System.Text;
using NElniorPackS;
using System.Net.Sockets;
using System.Threading.Tasks;
using NElniorPackS.NElniorMimeS;

namespace cbt_server.HttpIO
{
	internal sealed class Answer
	{
		private byte tries;
		private Anully entry;
		public static Mimetypes mime = new Mimetypes();
		public Answer (Anully entry)
		{
			this.entry = entry;
			this.tries = 0;
		}
		public void dispatch (Socket readyC)
		{
			bool isServerError = false;
			try
			{
				byte[] headers, data;
				string time = DateTime.Now.ToShortDateString();
				string address;
				string path = this.entry.path.Trim();

				switch (this.entry.method.Trim().ToLower())
				{
					case "get":
						if (path == "" || path == "/")
						{
							address = AcmySureServer.baseDir + @"\UI\home.html";
							if (File.Exists(address))
							{
								if (this.tries < 8)
								{
									FileStream homeStream = File.OpenRead(address);
									headers = Encoding.UTF8.GetBytes (
										this.entry.htWithVersion + " 200 Ok\r\n" +
										"Connection: close\r\n" +
										"Content-Length: " + homeStream.Length + "\r\n" +
										"Content-Language: en-US\r\n" +
										"Date: " + time + "\r\n" +
										"Content-Type: text/html\r\n" +
										"Server: NElniorS\r\n" +
										"\r\n"
									);
									
									readyC.Send(headers);
									
									data = new byte[1024]; // 1Kb
									while (homeStream.Read(data, 0, data.Length) != 0)
										readyC.Send(data);
									
									// cleaning
									homeStream.Close();
									homeStream.Dispose();
									readyC.Close();
									readyC.Dispose();
								}
								else
								{
									this.tries = 0;
									data = Encoding.UTF8.GetBytes("The source data is not free yet!");
									headers = Encoding.UTF8.GetBytes (
										this.entry.htWithVersion + " 504 Busy\r\n" +
										"Content-Length: " + data.Length + "\r\n" +
										"Connection: close\r\n" +
										"Content-Language: en-US\r\n" +
										"Date: " + time + "\r\n" +
										"Content-Type: text/txt\r\n" +
										"Server: NElniorS\r\n" +
										"\r\n"
									);
									readyC.Send(headers);
									readyC.Send(data);
									// cleaning
									readyC.Close();
									readyC.Dispose();
								}
							}
							else
							{
								isServerError = true;
								throw new Exception("The Main Source Server is not exist. Check the ../UI directory to analyze and fix it");
							}
						}
						else if (path == "/access" || path == "/access/")
						{
							address = AcmySureServer.baseDir + @"\UI\accessToAccount.html";
							if (File.Exists(address))
							{
								if (this.tries < 8)
								{
									FileStream homeStream = File.OpenRead(address);
									headers = Encoding.UTF8.GetBytes (
										this.entry.htWithVersion + " 307 Temporary Redirect\r\n" +
										"Connection: close\r\n" +
										"Content-Length: " + homeStream.Length + "\r\n" +
										"Content-Language: en-US\r\n" +
										"Date: " + time + "\r\n" +
										"Content-Type: text/html\r\n" +
										"Server: NElniorS\r\n" +
										"\r\n"
									);
									
									readyC.Send(headers);
									
									data = new byte[1024]; // 1Kb
									while (homeStream.Read(data, 0, data.Length) != 0)
										readyC.Send(data);
									
									// cleaning
									homeStream.Close();
									homeStream.Dispose();
									readyC.Close();
									readyC.Dispose();
								}
								else
								{
									this.tries = 0;
									data = Encoding.UTF8.GetBytes("The source data is not free yet!");
									headers = Encoding.UTF8.GetBytes (
										this.entry.htWithVersion + " 504 Busy\r\n" +
										"Content-Length: " + data.Length + "\r\n" +
										"Connection: close\r\n" +
										"Content-Language: en-US\r\n" +
										"Date: " + time + "\r\n" +
										"Conent-Type: text/txt\r\n" +
										"Server: NElniorS\r\n" +
										"\r\n"
									);
									readyC.Send(headers);
									readyC.Send(data);
									// cleaning
									readyC.Close();
									readyC.Dispose();
								}
							}
							else
							{
								isServerError = true;
								throw new Exception("If you want to access and you is the developer of AcmySure, check the ../UI directory to analyze and fix it");
							}
						}
						else if (File.Exists(AcmySureServer.baseDir + "/UI/sources" + path))
						{
							if (this.tries < 8)
							{
								FileStream dataStream = File.OpenRead(AcmySureServer.baseDir + "/UI/sources" + path);
								headers = Encoding.UTF8.GetBytes (
									this.entry.htWithVersion + " 200 Ok\r\n" +
									"Content-Length: " + dataStream.Length + "\r\n" +
									"Connection: close\r\n" +
									"Content-Language: en-US\r\n" +
									"Date: " + time + "\r\n" +
									"Content-Type: " + Answer.mime.getMime(path) + "\r\n" +
									"Server: NElniorS\r\n" +
									"\r\n"
								);
								
								readyC.Send(headers);

								data = new byte[1024]; // 1Kb
								while (dataStream.Read(data, 0, data.Length) != 0)
									readyC.Send(data);
								// cleaning
								dataStream.Close();
								dataStream.Dispose();
								readyC.Close();
								readyC.Dispose();
							}
							else
							{
								this.tries = 0;
								data = Encoding.UTF8.GetBytes("The source data is not free yet!");
								headers = Encoding.UTF8.GetBytes (
									this.entry.htWithVersion + " 504 Busy\r\n" +
									"Content-Length: " + data.Length + "\r\n" +
									"Connection: close\r\n" +
									"Content-Language: en-US\r\n" +
									"Date: " + time + "\r\n" +
									"Content-Type: text/txt\r\n" +
									"Server: NElniorS\r\n" +
									"\r\n"
								);
								readyC.Send(headers);
								readyC.Send(data);
								// cleaning
								readyC.Close();
								readyC.Dispose();
							}
						}
						else
						{
							string loc = AcmySureServer.baseDir + @"\UI\notAvailableInterface.html";
							if (File.Exists(loc))
							{
								if (this.tries < 8)
								{
									FileStream notValidInterfaceStream = File.OpenRead(loc);
									headers = Encoding.UTF8.GetBytes (
										this.entry.htWithVersion + " 404 Not available source\r\n" +
										"Content-Length: " + notValidInterfaceStream.Length + "\r\n" +
										"Content-Language: en-En\r\n" +
										"Date: " + time + "\r\n" +
										"Conent-Type: text/html\r\n" +
										"Server: NElniorS\r\n" +
										"\r\n"
									);
									
									readyC.Send(headers);
									
									data = new byte[1024]; // 1Kb
									while (notValidInterfaceStream.Read(data, 0, data.Length) != 0)
										readyC.Send(data);
									
									// cleaning
									notValidInterfaceStream.Close();
									notValidInterfaceStream.Dispose();
									readyC.Close();
									readyC.Dispose();
								}
								else
								{
									this.tries = 0;
									data = Encoding.UTF8.GetBytes("The source data is not free yet!");
									headers = Encoding.UTF8.GetBytes (
										this.entry.htWithVersion + " 508 Busy\r\n" +
										"Content-Length: " + data.Length + "\r\n" +
										"Content-Language: en-En\r\n" +
										"Date: " + time + "\r\n" +
										"Conent-Type: text/txt\r\n" +
										"Server: NElniorS\r\n" +
										"\r\n"
									);
									readyC.Send(headers);
									readyC.Send(data);
									// cleaning
									readyC.Close();
									readyC.Dispose();
								}
							}
							else
							{
								isServerError = true;
								throw new Exception("The source of server is not exist. Check the ../UI directory to analyze and fix it");
							}
						}
					break;
					
					default:
						string dateTime = DateTime.Now.ToShortDateString();
						string message = "Error (400): This is a bad request with \"" + this.entry.method + "\" Http method, please try with others methods like: \"Get\", etc..";
						data = Encoding.UTF8.GetBytes(message);

						headers = Encoding.UTF8.GetBytes (
							this.entry.htWithVersion + " 400 Bad Request\r\n" +
							"Content-Length: " + data.Length + "\r\n" +
							"Connection: close\r\n" +
							"Content-Language: en-US\r\n" +
							"Date: " + dateTime + "\r\n" +
							"Conent-Type: text/txt\r\n" +
							"Server: NElniorS\r\n" +
							"\r\n"
						);
						readyC.Send(headers);
						readyC.Send(data);
						readyC.Close();
						readyC.Dispose();
					break;
				}
			}
			catch (IOException e)
			{
				e.HelpLink = null;
				this.tries++;
				Task.Delay(350).Wait();
				this.dispatch(readyC);
			}
			catch (Exception e)
			{
				if (isServerError)
				{
					string dateTime = DateTime.Now.ToShortDateString();
					string message = "Internal Server Error is transcurred: " + e.Message;
					byte[] data = Encoding.UTF8.GetBytes(message);

					byte[] headers = Encoding.UTF8.GetBytes (
						this.entry.htWithVersion + " 500 Server Error\r\n" +
						"Content-Length: " + data.Length + "\r\n" +
						"Connection: close\r\n" +
						"Content-Language: en-US\r\n" +
						"Date: " + dateTime + "\r\n" +
						"Content-Type: text/txt\r\n" +
						"Server: NElniorS\r\n" +
						"\r\n"
					);
					readyC.Send(headers);
					readyC.Send(data);
					readyC.Close();
					readyC.Dispose();
				}
				else
				{
					readyC.Close();
					readyC.Dispose();
				}
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("0x001: {0}", e);
				Console.ResetColor();
			}
		}
	}
	public struct ServerInfo
	{
		public int connectionCount;
		public Socket[] connections;

		public void AddConnection (Socket connection)
		{
			Socket[] newSock = new Socket[1 + this.connections.Length];
			int index = 0;
			for (; index < this.connections.Length; index++)
				newSock[index] = this.connections[index];
			newSock[index] = connection;
			this.connections = newSock;
		}
	}
}