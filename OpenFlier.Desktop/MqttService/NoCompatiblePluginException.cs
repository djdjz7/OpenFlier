﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.MqttService
{

	[Serializable]
	public class NoCompatiblePluginException : Exception
	{
		public NoCompatiblePluginException() { }
		public NoCompatiblePluginException(string message) : base(message) { }
		public NoCompatiblePluginException(string message, Exception inner) : base(message, inner) { }
		protected NoCompatiblePluginException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
