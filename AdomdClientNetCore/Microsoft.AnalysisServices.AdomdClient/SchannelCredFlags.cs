using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SchannelCredFlags
	{
		public static readonly uint SCH_CRED_NO_SYSTEM_MAPPER = 2u;

		public static readonly uint SCH_CRED_NO_SERVERNAME_CHECK = 4u;

		public static readonly uint SCH_CRED_MANUAL_CRED_VALIDATION = 8u;

		public static readonly uint SCH_CRED_NO_DEFAULT_CREDS = 16u;

		public static readonly uint SCH_CRED_AUTO_CRED_VALIDATION = 32u;

		public static readonly uint SCH_CRED_USE_DEFAULT_CREDS = 64u;

		public static readonly uint SCH_CRED_DISABLE_RECONNECTS = 128u;

		public static readonly uint SCH_CRED_REVOCATION_CHECK_END_CERT = 256u;

		public static readonly uint SCH_CRED_REVOCATION_CHECK_CHAIN = 512u;

		public static readonly uint SCH_CRED_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT = 1024u;

		public static readonly uint SCH_CRED_IGNORE_NO_REVOCATION_CHECK = 2048u;

		public static readonly uint SCH_CRED_IGNORE_REVOCATION_OFFLINE = 4096u;

		public static readonly uint SCH_CRED_RESTRICTED_ROOTS = 8192u;

		public static readonly uint SCH_CRED_REVOCATION_CHECK_CACHE_ONLY = 16384u;

		public static readonly uint SCH_CRED_CACHE_ONLY_URL_RETRIEVAL = 32768u;

		public static readonly uint SCH_CRED_MEMORY_STORE_CERT = 65536u;

		public static readonly uint SCH_CRED_CACHE_ONLY_URL_RETRIEVAL_ON_CREATE = 131072u;

		public static readonly uint SCH_SEND_ROOT_CERT = 262144u;
	}
}
