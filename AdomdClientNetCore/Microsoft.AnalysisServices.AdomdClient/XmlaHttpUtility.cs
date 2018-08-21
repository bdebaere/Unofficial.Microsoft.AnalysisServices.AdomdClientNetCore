using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class XmlaHttpUtility
	{
		private class UrlDecoder
		{
			private int _bufferSize;

			private int _numChars;

			private char[] _charBuffer;

			private int _numBytes;

			private byte[] _byteBuffer;

			private Encoding _encoding;

			private void FlushBytes()
			{
				if (this._numBytes > 0)
				{
					this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
					this._numBytes = 0;
				}
			}

			internal UrlDecoder(int bufferSize, Encoding encoding)
			{
				this._bufferSize = bufferSize;
				this._encoding = encoding;
				this._charBuffer = new char[bufferSize];
			}

			internal void AddChar(char ch)
			{
				if (this._numBytes > 0)
				{
					this.FlushBytes();
				}
				this._charBuffer[this._numChars++] = ch;
			}

			internal void AddByte(byte b)
			{
				if (this._byteBuffer == null)
				{
					this._byteBuffer = new byte[this._bufferSize];
				}
				this._byteBuffer[this._numBytes++] = b;
			}

			internal string GetString()
			{
				if (this._numBytes > 0)
				{
					this.FlushBytes();
				}
				if (this._numChars > 0)
				{
					return new string(this._charBuffer, 0, this._numChars);
				}
				return string.Empty;
			}
		}

		[Serializable]
		internal class HttpValueCollection : NameValueCollection
		{
			internal HttpValueCollection() : base(StringComparer.OrdinalIgnoreCase)
			{
			}

			internal HttpValueCollection(string str, bool readOnly, bool urlencoded, Encoding encoding) : base(StringComparer.OrdinalIgnoreCase)
			{
				if (!string.IsNullOrEmpty(str))
				{
					this.FillFromString(str, urlencoded, encoding);
				}
				base.IsReadOnly = readOnly;
			}

			internal HttpValueCollection(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase)
			{
			}

			protected HttpValueCollection(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}

			internal void MakeReadOnly()
			{
				base.IsReadOnly = true;
			}

			internal void MakeReadWrite()
			{
				base.IsReadOnly = false;
			}

			internal void FillFromString(string s)
			{
				this.FillFromString(s, false, null);
			}

			internal void FillFromString(string s, bool urlencoded, Encoding encoding)
			{
				int num = (s != null) ? s.Length : 0;
				for (int i = 0; i < num; i++)
				{
					int num2 = i;
					int num3 = -1;
					while (i < num)
					{
						char c = s[i];
						if (c == '=')
						{
							if (num3 < 0)
							{
								num3 = i;
							}
						}
						else if (c == '&')
						{
							break;
						}
						i++;
					}
					string text = null;
					string text2;
					if (num3 >= 0)
					{
						text = s.Substring(num2, num3 - num2);
						text2 = s.Substring(num3 + 1, i - num3 - 1);
					}
					else
					{
						text2 = s.Substring(num2, i - num2);
					}
					if (urlencoded)
					{
						base.Add(XmlaHttpUtility.UrlDecode(text, encoding), XmlaHttpUtility.UrlDecode(text2, encoding));
					}
					else
					{
						base.Add(text, text2);
					}
					if (i == num - 1 && s[i] == '&')
					{
						base.Add(null, string.Empty);
					}
				}
			}

			internal void FillFromEncodedBytes(byte[] bytes, Encoding encoding)
			{
				int num = (bytes != null) ? bytes.Length : 0;
				for (int i = 0; i < num; i++)
				{
					int num2 = i;
					int num3 = -1;
					while (i < num)
					{
						byte b = bytes[i];
						if (b == 61)
						{
							if (num3 < 0)
							{
								num3 = i;
							}
						}
						else if (b == 38)
						{
							break;
						}
						i++;
					}
					string name;
					string value;
					if (num3 >= 0)
					{
						name = XmlaHttpUtility.UrlDecode(bytes, num2, num3 - num2, encoding);
						value = XmlaHttpUtility.UrlDecode(bytes, num3 + 1, i - num3 - 1, encoding);
					}
					else
					{
						name = null;
						value = XmlaHttpUtility.UrlDecode(bytes, num2, i - num2, encoding);
					}
					base.Add(name, value);
					if (i == num - 1 && bytes[i] == 38)
					{
						base.Add(null, string.Empty);
					}
				}
			}

			internal void Reset()
			{
				base.Clear();
			}

			public override string ToString()
			{
				return this.ToString(true);
			}

			internal virtual string ToString(bool urlencoded)
			{
				return this.ToString(urlencoded, null);
			}

			internal virtual string ToString(bool urlencoded, IDictionary excludeKeys)
			{
				int count = this.Count;
				if (count == 0)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < count; i++)
				{
					string text = this.GetKey(i);
					if (excludeKeys == null || text == null || excludeKeys[text] == null)
					{
						if (urlencoded)
						{
							text = XmlaHttpUtility.UrlEncodeUnicode(text);
						}
						string value = (text != null) ? (text + "=") : string.Empty;
						ArrayList arrayList = (ArrayList)base.BaseGet(i);
						int num = (arrayList != null) ? arrayList.Count : 0;
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append('&');
						}
						if (num == 1)
						{
							stringBuilder.Append(value);
							string value2 = (string)arrayList[0];
							if (urlencoded)
							{
								value2 = XmlaHttpUtility.UrlEncodeUnicode(value2);
							}
							stringBuilder.Append(value2);
						}
						else if (num == 0)
						{
							stringBuilder.Append(value);
						}
						else
						{
							for (int j = 0; j < num; j++)
							{
								if (j > 0)
								{
									stringBuilder.Append('&');
								}
								stringBuilder.Append(value);
								string value2 = (string)arrayList[j];
								if (urlencoded)
								{
									value2 = XmlaHttpUtility.UrlEncodeUnicode(value2);
								}
								stringBuilder.Append(value2);
							}
						}
					}
				}
				return stringBuilder.ToString();
			}
		}

		private static class HtmlEntities
		{
			private static string[] _entitiesList = new string[]
			{
				"\"-quot",
				"&-amp",
				"'-apos",
				"<-lt",
				">-gt",
				"\u00a0-nbsp",
				"¡-iexcl",
				"¢-cent",
				"£-pound",
				"¤-curren",
				"¥-yen",
				"¦-brvbar",
				"§-sect",
				"¨-uml",
				"©-copy",
				"ª-ordf",
				"«-laquo",
				"¬-not",
				"­-shy",
				"®-reg",
				"¯-macr",
				"°-deg",
				"±-plusmn",
				"²-sup2",
				"³-sup3",
				"´-acute",
				"µ-micro",
				"¶-para",
				"·-middot",
				"¸-cedil",
				"¹-sup1",
				"º-ordm",
				"»-raquo",
				"¼-frac14",
				"½-frac12",
				"¾-frac34",
				"¿-iquest",
				"À-Agrave",
				"Á-Aacute",
				"Â-Acirc",
				"Ã-Atilde",
				"Ä-Auml",
				"Å-Aring",
				"Æ-AElig",
				"Ç-Ccedil",
				"È-Egrave",
				"É-Eacute",
				"Ê-Ecirc",
				"Ë-Euml",
				"Ì-Igrave",
				"Í-Iacute",
				"Î-Icirc",
				"Ï-Iuml",
				"Ð-ETH",
				"Ñ-Ntilde",
				"Ò-Ograve",
				"Ó-Oacute",
				"Ô-Ocirc",
				"Õ-Otilde",
				"Ö-Ouml",
				"×-times",
				"Ø-Oslash",
				"Ù-Ugrave",
				"Ú-Uacute",
				"Û-Ucirc",
				"Ü-Uuml",
				"Ý-Yacute",
				"Þ-THORN",
				"ß-szlig",
				"à-agrave",
				"á-aacute",
				"â-acirc",
				"ã-atilde",
				"ä-auml",
				"å-aring",
				"æ-aelig",
				"ç-ccedil",
				"è-egrave",
				"é-eacute",
				"ê-ecirc",
				"ë-euml",
				"ì-igrave",
				"í-iacute",
				"î-icirc",
				"ï-iuml",
				"ð-eth",
				"ñ-ntilde",
				"ò-ograve",
				"ó-oacute",
				"ô-ocirc",
				"õ-otilde",
				"ö-ouml",
				"÷-divide",
				"ø-oslash",
				"ù-ugrave",
				"ú-uacute",
				"û-ucirc",
				"ü-uuml",
				"ý-yacute",
				"þ-thorn",
				"ÿ-yuml",
				"Œ-OElig",
				"œ-oelig",
				"Š-Scaron",
				"š-scaron",
				"Ÿ-Yuml",
				"ƒ-fnof",
				"ˆ-circ",
				"˜-tilde",
				"Α-Alpha",
				"Β-Beta",
				"Γ-Gamma",
				"Δ-Delta",
				"Ε-Epsilon",
				"Ζ-Zeta",
				"Η-Eta",
				"Θ-Theta",
				"Ι-Iota",
				"Κ-Kappa",
				"Λ-Lambda",
				"Μ-Mu",
				"Ν-Nu",
				"Ξ-Xi",
				"Ο-Omicron",
				"Π-Pi",
				"Ρ-Rho",
				"Σ-Sigma",
				"Τ-Tau",
				"Υ-Upsilon",
				"Φ-Phi",
				"Χ-Chi",
				"Ψ-Psi",
				"Ω-Omega",
				"α-alpha",
				"β-beta",
				"γ-gamma",
				"δ-delta",
				"ε-epsilon",
				"ζ-zeta",
				"η-eta",
				"θ-theta",
				"ι-iota",
				"κ-kappa",
				"λ-lambda",
				"μ-mu",
				"ν-nu",
				"ξ-xi",
				"ο-omicron",
				"π-pi",
				"ρ-rho",
				"ς-sigmaf",
				"σ-sigma",
				"τ-tau",
				"υ-upsilon",
				"φ-phi",
				"χ-chi",
				"ψ-psi",
				"ω-omega",
				"ϑ-thetasym",
				"ϒ-upsih",
				"ϖ-piv",
				"\u2002-ensp",
				"\u2003-emsp",
				"\u2009-thinsp",
				"‌-zwnj",
				"‍-zwj",
				"‎-lrm",
				"‏-rlm",
				"–-ndash",
				"—-mdash",
				"‘-lsquo",
				"’-rsquo",
				"‚-sbquo",
				"“-ldquo",
				"”-rdquo",
				"„-bdquo",
				"†-dagger",
				"‡-Dagger",
				"•-bull",
				"…-hellip",
				"‰-permil",
				"′-prime",
				"″-Prime",
				"‹-lsaquo",
				"›-rsaquo",
				"‾-oline",
				"⁄-frasl",
				"€-euro",
				"ℑ-image",
				"℘-weierp",
				"ℜ-real",
				"™-trade",
				"ℵ-alefsym",
				"←-larr",
				"↑-uarr",
				"→-rarr",
				"↓-darr",
				"↔-harr",
				"↵-crarr",
				"⇐-lArr",
				"⇑-uArr",
				"⇒-rArr",
				"⇓-dArr",
				"⇔-hArr",
				"∀-forall",
				"∂-part",
				"∃-exist",
				"∅-empty",
				"∇-nabla",
				"∈-isin",
				"∉-notin",
				"∋-ni",
				"∏-prod",
				"∑-sum",
				"−-minus",
				"∗-lowast",
				"√-radic",
				"∝-prop",
				"∞-infin",
				"∠-ang",
				"∧-and",
				"∨-or",
				"∩-cap",
				"∪-cup",
				"∫-int",
				"∴-there4",
				"∼-sim",
				"≅-cong",
				"≈-asymp",
				"≠-ne",
				"≡-equiv",
				"≤-le",
				"≥-ge",
				"⊂-sub",
				"⊃-sup",
				"⊄-nsub",
				"⊆-sube",
				"⊇-supe",
				"⊕-oplus",
				"⊗-otimes",
				"⊥-perp",
				"⋅-sdot",
				"⌈-lceil",
				"⌉-rceil",
				"⌊-lfloor",
				"⌋-rfloor",
				"〈-lang",
				"〉-rang",
				"◊-loz",
				"♠-spades",
				"♣-clubs",
				"♥-hearts",
				"♦-diams"
			};

			private static Dictionary<string, char> _lookupTable = XmlaHttpUtility.HtmlEntities.GenerateLookupTable();

			private static Dictionary<string, char> GenerateLookupTable()
			{
				Dictionary<string, char> dictionary = new Dictionary<string, char>(StringComparer.Ordinal);
				string[] entitiesList = XmlaHttpUtility.HtmlEntities._entitiesList;
				for (int i = 0; i < entitiesList.Length; i++)
				{
					string text = entitiesList[i];
					dictionary.Add(text.Substring(2), text[0]);
				}
				return dictionary;
			}

			public static char Lookup(string entity)
			{
				char result;
				XmlaHttpUtility.HtmlEntities._lookupTable.TryGetValue(entity, out result);
				return result;
			}
		}

		private static char[] _htmlEntityEndingChars = new char[]
		{
			';',
			'&'
		};

		public static string AddSessionToUrl(string url, string session)
		{
			return XmlaHttpUtility.AddValueToUrlQuery(url, "SessionId", session);
		}

		private static string AddValueToUrlQuery(string url, string key, string value)
		{
			UriBuilder uriBuilder = new UriBuilder(url);
			NameValueCollection nameValueCollection = XmlaHttpUtility.ParseQueryString(uriBuilder.Query);
			nameValueCollection[key] = value;
			uriBuilder.Query = nameValueCollection.ToString();
			return uriBuilder.ToString();
		}

		public static string ExtractSessionFromUrl(string url, out string session)
		{
			return XmlaHttpUtility.ExtractValueFromUrlQuery(url, "SessionId", out session);
		}

		private static string ExtractValueFromUrlQuery(string url, string key, out string value)
		{
			UriBuilder uriBuilder = new UriBuilder(url);
			NameValueCollection nameValueCollection = XmlaHttpUtility.ParseQueryString(uriBuilder.Query);
			value = nameValueCollection.Get(key);
			if (value != null)
			{
				nameValueCollection.Remove("SessionId");
				uriBuilder.Query = nameValueCollection.ToString();
				return uriBuilder.ToString();
			}
			return url;
		}

		public static NameValueCollection ParseQueryString(string query)
		{
			return XmlaHttpUtility.ParseQueryString(query, Encoding.UTF8);
		}

		public static NameValueCollection ParseQueryString(string query, Encoding encoding)
		{
			return XmlaHttpUtility.ParseQueryString(query, encoding, true);
		}

		public static NameValueCollection ParseQueryString(string query, Encoding encoding, bool urlEncoded)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (query.Length > 0 && query[0] == '?')
			{
				query = query.Substring(1);
			}
			return new XmlaHttpUtility.HttpValueCollection(query, false, urlEncoded, encoding);
		}

		private static byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
		{
			byte[] array = XmlaHttpUtility.UrlEncode(bytes, offset, count);
			if (!alwaysCreateNewReturnValue || array == null || array != bytes)
			{
				return array;
			}
			return (byte[])array.Clone();
		}

		private static byte[] UrlEncode(byte[] bytes, int offset, int count)
		{
			if (!XmlaHttpUtility.ValidateUrlEncodingParameters(bytes, offset, count))
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				char c = (char)bytes[offset + i];
				if (c == ' ')
				{
					num++;
				}
				else if (!XmlaHttpUtility.IsUrlSafeChar(c))
				{
					num2++;
				}
			}
			if (num == 0 && num2 == 0)
			{
				return bytes;
			}
			byte[] array = new byte[count + num2 * 2];
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				byte b = bytes[offset + j];
				char c2 = (char)b;
				if (XmlaHttpUtility.IsUrlSafeChar(c2))
				{
					array[num3++] = b;
				}
				else if (c2 == ' ')
				{
					array[num3++] = 43;
				}
				else
				{
					array[num3++] = 37;
					array[num3++] = (byte)XmlaHttpUtility.IntToHex(b >> 4 & 15);
					array[num3++] = (byte)XmlaHttpUtility.IntToHex((int)(b & 15));
				}
			}
			return array;
		}

		private static string UrlEncodeNonAscii(string str, Encoding e)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			if (e == null)
			{
				e = Encoding.UTF8;
			}
			byte[] bytes = e.GetBytes(str);
			byte[] bytes2 = XmlaHttpUtility.UrlEncodeNonAscii(bytes, 0, bytes.Length, false);
			return Encoding.ASCII.GetString(bytes2);
		}

		private static byte[] UrlEncodeNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
		{
			if (!XmlaHttpUtility.ValidateUrlEncodingParameters(bytes, offset, count))
			{
				return null;
			}
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (XmlaHttpUtility.IsNonAsciiByte(bytes[offset + i]))
				{
					num++;
				}
			}
			if (!alwaysCreateNewReturnValue && num == 0)
			{
				return bytes;
			}
			byte[] array = new byte[count + num * 2];
			int num2 = 0;
			for (int j = 0; j < count; j++)
			{
				byte b = bytes[offset + j];
				if (XmlaHttpUtility.IsNonAsciiByte(b))
				{
					array[num2++] = 37;
					array[num2++] = (byte)XmlaHttpUtility.IntToHex(b >> 4 & 15);
					array[num2++] = (byte)XmlaHttpUtility.IntToHex((int)(b & 15));
				}
				else
				{
					array[num2++] = b;
				}
			}
			return array;
		}

		public static string UrlEncode(string str)
		{
			if (str == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlEncode(str, Encoding.UTF8);
		}

		public static string UrlEncode(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			return Encoding.ASCII.GetString(XmlaHttpUtility.UrlEncodeToBytes(str, e));
		}

		public static string UrlEncode(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return Encoding.ASCII.GetString(XmlaHttpUtility.UrlEncodeToBytes(bytes));
		}

		public static byte[] UrlEncodeToBytes(string str)
		{
			if (str == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlEncodeToBytes(str, Encoding.UTF8);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);
		}

		public static byte[] UrlEncodeToBytes(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			byte[] bytes = e.GetBytes(str);
			return XmlaHttpUtility.UrlEncode(bytes, 0, bytes.Length, false);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			return XmlaHttpUtility.UrlEncode(bytes, offset, count, true);
		}

		public static string UrlPathEncode(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			int num = str.IndexOf('?');
			if (num >= 0)
			{
				return XmlaHttpUtility.UrlPathEncode(str.Substring(0, num)) + str.Substring(num);
			}
			return XmlaHttpUtility.UrlEncodeSpaces(XmlaHttpUtility.UrlEncodeNonAscii(str, Encoding.UTF8));
		}

		public static string UrlEncodeUnicode(string value)
		{
			if (value == null)
			{
				return null;
			}
			int length = value.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if ((c & 'ﾀ') == '\0')
				{
					if (XmlaHttpUtility.IsUrlSafeChar(c))
					{
						stringBuilder.Append(c);
					}
					else if (c == ' ')
					{
						stringBuilder.Append('+');
					}
					else
					{
						stringBuilder.Append('%');
						stringBuilder.Append(XmlaHttpUtility.IntToHex((int)(c >> 4 & '\u000f')));
						stringBuilder.Append(XmlaHttpUtility.IntToHex((int)(c & '\u000f')));
					}
				}
				else
				{
					stringBuilder.Append("%u");
					stringBuilder.Append(XmlaHttpUtility.IntToHex((int)(c >> 12 & '\u000f')));
					stringBuilder.Append(XmlaHttpUtility.IntToHex((int)(c >> 8 & '\u000f')));
					stringBuilder.Append(XmlaHttpUtility.IntToHex((int)(c >> 4 & '\u000f')));
					stringBuilder.Append(XmlaHttpUtility.IntToHex((int)(c & '\u000f')));
				}
			}
			return stringBuilder.ToString();
		}

		private static string UrlDecodeInternal(string value, Encoding encoding)
		{
			if (value == null)
			{
				return null;
			}
			int length = value.Length;
			XmlaHttpUtility.UrlDecoder urlDecoder = new XmlaHttpUtility.UrlDecoder(length, encoding);
			int i = 0;
			while (i < length)
			{
				char c = value[i];
				if (c == '+')
				{
					c = ' ';
					goto IL_10B;
				}
				if (c != '%' || i >= length - 2)
				{
					goto IL_10B;
				}
				if (value[i + 1] == 'u' && i < length - 5)
				{
					int num = XmlaHttpUtility.HexToInt(value[i + 2]);
					int num2 = XmlaHttpUtility.HexToInt(value[i + 3]);
					int num3 = XmlaHttpUtility.HexToInt(value[i + 4]);
					int num4 = XmlaHttpUtility.HexToInt(value[i + 5]);
					if (num < 0 || num2 < 0 || num3 < 0 || num4 < 0)
					{
						goto IL_10B;
					}
					c = (char)(num << 12 | num2 << 8 | num3 << 4 | num4);
					i += 5;
					urlDecoder.AddChar(c);
				}
				else
				{
					int num5 = XmlaHttpUtility.HexToInt(value[i + 1]);
					int num6 = XmlaHttpUtility.HexToInt(value[i + 2]);
					if (num5 < 0 || num6 < 0)
					{
						goto IL_10B;
					}
					byte b = (byte)(num5 << 4 | num6);
					i += 2;
					urlDecoder.AddByte(b);
				}
				IL_125:
				i++;
				continue;
				IL_10B:
				if ((c & 'ﾀ') == '\0')
				{
					urlDecoder.AddByte((byte)c);
					goto IL_125;
				}
				urlDecoder.AddChar(c);
				goto IL_125;
			}
			return urlDecoder.GetString();
		}

		private static byte[] UrlDecodeInternal(byte[] bytes, int offset, int count)
		{
			if (!XmlaHttpUtility.ValidateUrlEncodingParameters(bytes, offset, count))
			{
				return null;
			}
			int num = 0;
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				int num2 = offset + i;
				byte b = bytes[num2];
				if (b == 43)
				{
					b = 32;
				}
				else if (b == 37 && i < count - 2)
				{
					int num3 = XmlaHttpUtility.HexToInt((char)bytes[num2 + 1]);
					int num4 = XmlaHttpUtility.HexToInt((char)bytes[num2 + 2]);
					if (num3 >= 0 && num4 >= 0)
					{
						b = (byte)(num3 << 4 | num4);
						i += 2;
					}
				}
				array[num++] = b;
			}
			if (num < array.Length)
			{
				byte[] array2 = new byte[num];
				Array.Copy(array, array2, num);
				array = array2;
			}
			return array;
		}

		private static string UrlDecodeInternal(byte[] bytes, int offset, int count, Encoding encoding)
		{
			if (!XmlaHttpUtility.ValidateUrlEncodingParameters(bytes, offset, count))
			{
				return null;
			}
			XmlaHttpUtility.UrlDecoder urlDecoder = new XmlaHttpUtility.UrlDecoder(count, encoding);
			int i = 0;
			while (i < count)
			{
				int num = offset + i;
				byte b = bytes[num];
				if (b == 43)
				{
					b = 32;
					goto IL_E6;
				}
				if (b != 37 || i >= count - 2)
				{
					goto IL_E6;
				}
				if (bytes[num + 1] == 117 && i < count - 5)
				{
					int num2 = XmlaHttpUtility.HexToInt((char)bytes[num + 2]);
					int num3 = XmlaHttpUtility.HexToInt((char)bytes[num + 3]);
					int num4 = XmlaHttpUtility.HexToInt((char)bytes[num + 4]);
					int num5 = XmlaHttpUtility.HexToInt((char)bytes[num + 5]);
					if (num2 < 0 || num3 < 0 || num4 < 0 || num5 < 0)
					{
						goto IL_E6;
					}
					char ch = (char)(num2 << 12 | num3 << 8 | num4 << 4 | num5);
					i += 5;
					urlDecoder.AddChar(ch);
				}
				else
				{
					int num6 = XmlaHttpUtility.HexToInt((char)bytes[num + 1]);
					int num7 = XmlaHttpUtility.HexToInt((char)bytes[num + 2]);
					if (num6 >= 0 && num7 >= 0)
					{
						b = (byte)(num6 << 4 | num7);
						i += 2;
						goto IL_E6;
					}
					goto IL_E6;
				}
				IL_ED:
				i++;
				continue;
				IL_E6:
				urlDecoder.AddByte(b);
				goto IL_ED;
			}
			return urlDecoder.GetString();
		}

		public static string UrlDecode(string str)
		{
			if (str == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlDecode(str, Encoding.UTF8);
		}

		public static string UrlDecode(string str, Encoding e)
		{
			return XmlaHttpUtility.UrlDecodeInternal(str, e);
		}

		public static string UrlDecode(byte[] bytes, Encoding e)
		{
			if (bytes == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlDecode(bytes, 0, bytes.Length, e);
		}

		public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
		{
			return XmlaHttpUtility.UrlDecodeInternal(bytes, offset, count, e);
		}

		public static byte[] UrlDecodeToBytes(string str)
		{
			if (str == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlDecodeToBytes(str, Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlDecodeToBytes(e.GetBytes(str));
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return XmlaHttpUtility.UrlDecodeToBytes(bytes, 0, (bytes != null) ? bytes.Length : 0);
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			return XmlaHttpUtility.UrlDecodeInternal(bytes, offset, count);
		}

		public static int HexToInt(char h)
		{
			if (h >= '0' && h <= '9')
			{
				return (int)(h - '0');
			}
			if (h >= 'a' && h <= 'f')
			{
				return (int)(h - 'a' + '\n');
			}
			if (h < 'A' || h > 'F')
			{
				return -1;
			}
			return (int)(h - 'A' + '\n');
		}

		public static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		public static bool IsUrlSafeChar(char ch)
		{
			if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
			{
				return true;
			}
			if (ch != '!')
			{
				switch (ch)
				{
				case '(':
				case ')':
				case '*':
				case '-':
				case '.':
					return true;
				case '+':
				case ',':
					break;
				default:
					if (ch == '_')
					{
						return true;
					}
					break;
				}
				return false;
			}
			return true;
		}

		internal static string UrlEncodeSpaces(string str)
		{
			if (str != null && str.IndexOf(' ') >= 0)
			{
				str = str.Replace(" ", "%20");
			}
			return str;
		}

		private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
		{
			if (bytes == null && count == 0)
			{
				return false;
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (offset < 0 || offset > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return true;
		}

		private static bool IsNonAsciiByte(byte b)
		{
			return b >= 127 || b < 32;
		}

		public static string HtmlDecode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			if (value.IndexOf('&') < 0)
			{
				return value;
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlaHttpUtility.HtmlDecode(value, stringWriter);
			return stringWriter.ToString();
		}

		public static void HtmlDecode(string value, TextWriter output)
		{
			if (value == null)
			{
				return;
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (value.IndexOf('&') < 0)
			{
				output.Write(value);
				return;
			}
			int length = value.Length;
			int i = 0;
			while (i < length)
			{
				char c = value[i];
				if (c != '&')
				{
					goto IL_110;
				}
				int num = value.IndexOfAny(XmlaHttpUtility._htmlEntityEndingChars, i + 1);
				if (num <= 0 || value[num] != ';')
				{
					goto IL_110;
				}
				string text = value.Substring(i + 1, num - i - 1);
				if (text.Length > 1 && text[0] == '#')
				{
					ushort num2;
					if (text[1] == 'x' || text[1] == 'X')
					{
						ushort.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out num2);
					}
					else
					{
						ushort.TryParse(text.Substring(1), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2);
					}
					if (num2 != 0)
					{
						c = (char)num2;
						i = num;
						goto IL_110;
					}
					goto IL_110;
				}
				else
				{
					i = num;
					char c2 = XmlaHttpUtility.HtmlEntities.Lookup(text);
					if (c2 != '\0')
					{
						c = c2;
						goto IL_110;
					}
					output.Write('&');
					output.Write(text);
					output.Write(';');
				}
				IL_117:
				i++;
				continue;
				IL_110:
				output.Write(c);
				goto IL_117;
			}
		}
	}
}
