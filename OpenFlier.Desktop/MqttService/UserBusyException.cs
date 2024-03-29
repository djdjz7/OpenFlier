﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.MqttService
{

	[Serializable]
	public class UserBusyException : Exception
	{
		public UserBusyException() { }
		public UserBusyException(string message) : base(message) { }
		public UserBusyException(string message, Exception inner) : base(message, inner) { }
		protected UserBusyException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
