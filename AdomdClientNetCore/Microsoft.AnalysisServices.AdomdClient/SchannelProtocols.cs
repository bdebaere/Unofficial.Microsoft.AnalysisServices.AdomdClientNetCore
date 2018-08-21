using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SchannelProtocols
	{
		public static readonly uint SP_PROT_PCT1_SERVER = 1u;

		public static readonly uint SP_PROT_PCT1_CLIENT = 2u;

		public static readonly uint SP_PROT_PCT1 = SchannelProtocols.SP_PROT_PCT1_SERVER | SchannelProtocols.SP_PROT_PCT1_CLIENT;

		public static readonly uint SP_PROT_SSL2_SERVER = 4u;

		public static readonly uint SP_PROT_SSL2_CLIENT = 8u;

		public static readonly uint SP_PROT_SSL2 = SchannelProtocols.SP_PROT_SSL2_SERVER | SchannelProtocols.SP_PROT_SSL2_CLIENT;

		public static readonly uint SP_PROT_SSL3_SERVER = 16u;

		public static readonly uint SP_PROT_SSL3_CLIENT = 32u;

		public static readonly uint SP_PROT_SSL3 = SchannelProtocols.SP_PROT_SSL3_SERVER | SchannelProtocols.SP_PROT_SSL3_CLIENT;

		public static readonly uint SP_PROT_TLS1_SERVER = 64u;

		public static readonly uint SP_PROT_TLS1_CLIENT = 128u;

		public static readonly uint SP_PROT_TLS1 = SchannelProtocols.SP_PROT_TLS1_SERVER | SchannelProtocols.SP_PROT_TLS1_CLIENT;

		public static readonly uint SP_PROT_SSL3TLS1_CLIENTS = SchannelProtocols.SP_PROT_TLS1_CLIENT | SchannelProtocols.SP_PROT_SSL3_CLIENT;

		public static readonly uint SP_PROT_SSL3TLS1_SERVERS = SchannelProtocols.SP_PROT_TLS1_SERVER | SchannelProtocols.SP_PROT_SSL3_SERVER;

		public static readonly uint SP_PROT_SSL3TLS1 = SchannelProtocols.SP_PROT_SSL3 | SchannelProtocols.SP_PROT_TLS1;

		public static readonly uint SP_PROT_UNI_SERVER = 1073741824u;

		public static readonly uint SP_PROT_UNI_CLIENT = 2147483648u;

		public static readonly uint SP_PROT_UNI = SchannelProtocols.SP_PROT_UNI_SERVER | SchannelProtocols.SP_PROT_UNI_CLIENT;

		public static readonly uint SP_PROT_ALL = 4294967295u;

		public static readonly uint SP_PROT_NONE = 0u;

		public static readonly uint SP_PROT_CLIENTS = SchannelProtocols.SP_PROT_PCT1_CLIENT | SchannelProtocols.SP_PROT_SSL2_CLIENT | SchannelProtocols.SP_PROT_SSL3_CLIENT | SchannelProtocols.SP_PROT_UNI_CLIENT | SchannelProtocols.SP_PROT_TLS1_CLIENT;

		public static readonly uint SP_PROT_SERVERS = SchannelProtocols.SP_PROT_PCT1_SERVER | SchannelProtocols.SP_PROT_SSL2_SERVER | SchannelProtocols.SP_PROT_SSL3_SERVER | SchannelProtocols.SP_PROT_UNI_SERVER | SchannelProtocols.SP_PROT_TLS1_SERVER;

		public static readonly uint SP_PROT_TLS1_0_SERVER = SchannelProtocols.SP_PROT_TLS1_SERVER;

		public static readonly uint SP_PROT_TLS1_0_CLIENT = SchannelProtocols.SP_PROT_TLS1_CLIENT;

		public static readonly uint SP_PROT_TLS1_0 = SchannelProtocols.SP_PROT_TLS1_0_SERVER | SchannelProtocols.SP_PROT_TLS1_0_CLIENT;

		public static readonly uint SP_PROT_TLS1_1_SERVER = 256u;

		public static readonly uint SP_PROT_TLS1_1_CLIENT = 512u;

		public static readonly uint SP_PROT_TLS1_1 = SchannelProtocols.SP_PROT_TLS1_1_SERVER | SchannelProtocols.SP_PROT_TLS1_1_CLIENT;

		public static readonly uint SP_PROT_TLS1_2_SERVER = 1024u;

		public static readonly uint SP_PROT_TLS1_2_CLIENT = 2048u;

		public static readonly uint SP_PROT_TLS1_2 = SchannelProtocols.SP_PROT_TLS1_2_SERVER | SchannelProtocols.SP_PROT_TLS1_2_CLIENT;

		public static readonly uint SP_PROT_TLS1_1PLUS_SERVER = SchannelProtocols.SP_PROT_TLS1_1_SERVER | SchannelProtocols.SP_PROT_TLS1_2_SERVER;

		public static readonly uint SP_PROT_TLS1_1PLUS_CLIENT = SchannelProtocols.SP_PROT_TLS1_1_CLIENT | SchannelProtocols.SP_PROT_TLS1_2_CLIENT;

		public static readonly uint SP_PROT_TLS1_1PLUS = SchannelProtocols.SP_PROT_TLS1_1PLUS_SERVER | SchannelProtocols.SP_PROT_TLS1_1PLUS_CLIENT;

		public static readonly uint SP_PROT_TLS1_X_SERVER = SchannelProtocols.SP_PROT_TLS1_0_SERVER | SchannelProtocols.SP_PROT_TLS1_1_SERVER | SchannelProtocols.SP_PROT_TLS1_2_SERVER;

		public static readonly uint SP_PROT_TLS1_X_CLIENT = SchannelProtocols.SP_PROT_TLS1_0_CLIENT | SchannelProtocols.SP_PROT_TLS1_1_CLIENT | SchannelProtocols.SP_PROT_TLS1_2_CLIENT;

		public static readonly uint SP_PROT_TLS1_X = SchannelProtocols.SP_PROT_TLS1_X_SERVER | SchannelProtocols.SP_PROT_TLS1_X_CLIENT;

		public static readonly uint SP_PROT_SSL3TLS1_X_CLIENTS = SchannelProtocols.SP_PROT_TLS1_X_CLIENT | SchannelProtocols.SP_PROT_SSL3_CLIENT;

		public static readonly uint SP_PROT_SSL3TLS1_X_SERVERS = SchannelProtocols.SP_PROT_TLS1_X_SERVER | SchannelProtocols.SP_PROT_SSL3_SERVER;

		public static readonly uint SP_PROT_SSL3TLS1_X = SchannelProtocols.SP_PROT_SSL3 | SchannelProtocols.SP_PROT_TLS1_X;

		public static readonly uint SP_PROT_X_CLIENTS = SchannelProtocols.SP_PROT_CLIENTS | SchannelProtocols.SP_PROT_TLS1_X_CLIENT;

		public static readonly uint SP_PROT_X_SERVERS = SchannelProtocols.SP_PROT_SERVERS | SchannelProtocols.SP_PROT_TLS1_X_SERVER;
	}
}
