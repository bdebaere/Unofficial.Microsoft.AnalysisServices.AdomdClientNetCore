using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class AdalRuntimeLibrary
	{
		internal class CacheLocalStorage
		{
			private AdalRuntimeLibrary adal = AdalRuntimeLibrary.Instance;

			private string cacheFilePath;

			private object cacheFileLock;

			private string dataSource;

			private object cache;

			private bool cleanCacheInProgress;

			internal CacheLocalStorage(object cache, string dataSource)
			{
				this.cache = cache;
				if (!Directory.Exists(AdalRuntimeLibrary.AdalCacheDirectory))
				{
					Directory.CreateDirectory(AdalRuntimeLibrary.AdalCacheDirectory);
				}
				string mD5Hash = AdalRuntimeLibrary.CacheLocalStorage.GetMD5Hash(dataSource);
				this.cacheFilePath = string.Format("{0}\\{1}_{2}.dat", AdalRuntimeLibrary.AdalCacheDirectory, "managed_asazure", mD5Hash);
				this.cacheFileLock = ASAzureUtility.GetAppDomainLock("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_ADAL_CACHE_" + mD5Hash);
				this.dataSource = dataSource;
				Delegate value = Delegate.CreateDelegate(this.adal.TokenCacheNotificationT, this, AdalRuntimeLibrary.CacheLocalStorage.GetMethodInfo(new Action<object>(this.BeforeAccessNotification)));
				Delegate value2 = Delegate.CreateDelegate(this.adal.TokenCacheNotificationT, this, AdalRuntimeLibrary.CacheLocalStorage.GetMethodInfo(new Action<object>(this.AfterAccessNotification)));
				Delegate value3 = Delegate.CreateDelegate(this.adal.TokenCacheNotificationT, this, AdalRuntimeLibrary.CacheLocalStorage.GetMethodInfo(new Action<object>(this.BeforeWriteNotification)));
				this.adal.BeforeAccessP.SetValue(cache, value, null);
				this.adal.AfterAccessP.SetValue(cache, value2, null);
				this.adal.BeforeWriteP.SetValue(cache, value3, null);
			}

			public void BeforeAccessNotification(object args)
			{
				lock (this.cacheFileLock)
				{
					if (File.Exists(this.cacheFilePath))
					{
						byte[] array = this.ReadProtectedData(this.cacheFilePath);
						if (array != null)
						{
							//byte[] array2 = ProtectedData.Unprotect(array, null, DataProtectionScope.CurrentUser);
							this.adal.DeserializeM.Invoke(this.cache, new object[]
							{
								//array2
							});
						}
					}
				}
			}

			public void AfterAccessNotification(object args)
			{
				if ((bool)this.adal.HasStateChangedP.GetValue(this.cache, null))
				{
					lock (this.cacheFileLock)
					{
						byte[] userData = (byte[])this.adal.SerializeM.Invoke(this.cache, new object[0]);
						//byte[] protectedData = ProtectedData.Protect(userData, null, DataProtectionScope.CurrentUser);
						//this.WriteCacheContent(this.cacheFilePath, this.dataSource, protectedData);
						this.adal.HasStateChangedP.SetValue(this.cache, false, null);
					}
				}
			}

			public void BeforeWriteNotification(object args)
			{
				if (this.cleanCacheInProgress)
				{
					return;
				}
				try
				{
					this.cleanCacheInProgress = true;
					this.adal.ClearM.Invoke(this.cache, null);
				}
				finally
				{
					this.cleanCacheInProgress = false;
				}
			}

			private static MethodInfo GetMethodInfo(Action<object> a)
			{
				return a.Method;
			}

			private static string GetMD5Hash(string input)
			{
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] bytes = new UnicodeEncoding().GetBytes(input);
				byte[] value = mD5CryptoServiceProvider.ComputeHash(bytes);
				return BitConverter.ToString(value).Replace("-", string.Empty).ToLowerInvariant();
			}

			private byte[] ReadProtectedData(string cacheFilePath)
			{
				CacheContent cacheContent = new CacheContent();
				cacheContent.Deserialize(cacheFilePath);
				return cacheContent.GetProtectedData();
			}

			private void WriteCacheContent(string cacheFilePath, string dataSource, byte[] protectedData)
			{
				CacheContent cacheContent = new CacheContent(dataSource, protectedData);
				cacheContent.Serialize(cacheFilePath);
			}
		}

		internal class Usage : IDisposable
		{
			internal Usage()
			{
				AdalRuntimeLibrary.BeforeAdalUsage();
			}

			public void Dispose()
			{
				AdalRuntimeLibrary.AfterAdalUsage();
			}
		}

		private const string AppDomainAdalLoadingLockName = "MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_ADAL_LOADING_LOCK";

		private const string AppDomainAdalAssembliesKey = "MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_LOADED_ADAL_ASSEMBLIES";

		private static string AdalCacheDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\AADCacheOM";

		private static volatile AdalRuntimeLibrary instance;

		private static object SyncLock = new object();

		private static int CurrentThreads;

		private static string[] AdalAssemblyNames = new string[]
		{
			"Microsoft.IdentityModel.Clients.ActiveDirectory.dll",
			"Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll"
		};

		internal Type AuthenticationContextT;

		internal Type PromptBehaviorT;

		internal Type AuthenticationResultT;

		internal object PromptBehaviorAutoV;

		internal MethodInfo AcquireTokenM;

		internal MethodInfo AcquireTokenByRefreshTokenM;

		internal MethodInfo AcquireTokenUserIdM;

		internal MethodInfo AcquireTokenCredentialsM;

		internal PropertyInfo TokenCacheP;

		internal PropertyInfo AccessTokenP;

		internal PropertyInfo RefreshTokenP;

		internal PropertyInfo ExpiresOnP;

		internal Type TokenCacheT;

		internal Type TokenCacheNotificationT;

		internal PropertyInfo BeforeAccessP;

		internal PropertyInfo AfterAccessP;

		internal PropertyInfo BeforeWriteP;

		internal PropertyInfo HasStateChangedP;

		internal MethodInfo DeserializeM;

		internal MethodInfo SerializeM;

		internal MethodInfo ReadItemsM;

		internal MethodInfo ClearM;

		internal Type TokenCacheItemT;

		internal PropertyInfo AuthorityP;

		internal PropertyInfo ClientIdP;

		internal PropertyInfo DisplayableIdP;

		internal Type UserCredentialT;

		internal Type UserIdentifierT;

		internal Type UserIdentifierTypeT;

		internal object OptionalDisplayableIdV;

		private static Dictionary<string, Assembly> AdalAssembliesDictionary = new Dictionary<string, Assembly>();

		internal static AdalRuntimeLibrary Instance
		{
			get
			{
				if (AdalRuntimeLibrary.instance == null)
				{
					lock (AdalRuntimeLibrary.SyncLock)
					{
						if (AdalRuntimeLibrary.instance == null)
						{
							AdalRuntimeLibrary.instance = new AdalRuntimeLibrary();
						}
					}
				}
				return AdalRuntimeLibrary.instance;
			}
		}

		internal static void BeforeAdalUsage()
		{
			lock (AdalRuntimeLibrary.SyncLock)
			{
				AdalRuntimeLibrary.CurrentThreads++;
				if (AdalRuntimeLibrary.CurrentThreads == 1)
				{
					AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AdalRuntimeLibrary.OnResolveAssembly);
				}
			}
		}

		internal static void AfterAdalUsage()
		{
			lock (AdalRuntimeLibrary.SyncLock)
			{
				AdalRuntimeLibrary.CurrentThreads--;
				if (AdalRuntimeLibrary.CurrentThreads == 0)
				{
					AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AdalRuntimeLibrary.OnResolveAssembly);
				}
			}
		}

		private AdalRuntimeLibrary()
		{
			Assembly assembly = AdalRuntimeLibrary.LoadAdalAssembly();
			this.LoadUserAuthTypes(assembly);
			this.LoadAuthenticationContextTypes(assembly);
			this.LoadTokenCacheTypes(assembly);
		}

		private static Assembly OnResolveAssembly(object sender, ResolveEventArgs bargs)
		{
			if (!AdalRuntimeLibrary.AdalAssembliesDictionary.ContainsKey(bargs.Name))
			{
				return null;
			}
			return AdalRuntimeLibrary.AdalAssembliesDictionary[bargs.Name];
		}

		private static Assembly LoadAdalAssembly()
		{
			List<Assembly> list;
			lock (ASAzureUtility.GetAppDomainLock("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_ADAL_LOADING_LOCK"))
			{
				list = (List<Assembly>)AppDomain.CurrentDomain.GetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_LOADED_ADAL_ASSEMBLIES");
				if (list != null)
				{
					list.ForEach(delegate(Assembly assembly)
					{
						AdalRuntimeLibrary.AdalAssembliesDictionary.Add(assembly.FullName, assembly);
					});
				}
				else
				{
					list = new List<Assembly>();
					string[] adalAssemblyNames = AdalRuntimeLibrary.AdalAssemblyNames;
					for (int i = 0; i < adalAssemblyNames.Length; i++)
					{
						string name = adalAssemblyNames[i];
						Assembly assembly2 = AdalRuntimeLibrary.LoadAssemblyFromLocalResource(name);
						list.Add(assembly2);
						AdalRuntimeLibrary.AdalAssembliesDictionary.Add(assembly2.FullName, assembly2);
					}
					AppDomain.CurrentDomain.SetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_LOADED_ADAL_ASSEMBLIES", list);
				}
			}
			return list.First<Assembly>();
		}

		private static Assembly LoadAssemblyFromLocalResource(string name)
		{
			Assembly result;
			using (Stream resourceAsStream = AdalRuntimeLibrary.GetResourceAsStream(name))
			{
				byte[] array = new byte[resourceAsStream.Length];
				resourceAsStream.Read(array, 0, array.Length);
				result = Assembly.Load(array);
			}
			return result;
		}

		private void LoadUserAuthTypes(Assembly assembly)
		{
			this.UserCredentialT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.UserCredential");
			this.UserIdentifierT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier");
			this.UserIdentifierTypeT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifierType");
			this.OptionalDisplayableIdV = this.LoadAndValidateEnumValue(this.UserIdentifierTypeT, "OptionalDisplayableId");
		}

		private void LoadAuthenticationContextTypes(Assembly assembly)
		{
			this.AuthenticationContextT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext");
			this.PromptBehaviorT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior");
			this.AuthenticationResultT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult");
			this.AcquireTokenM = this.LoadAndValidateMethod(this.AuthenticationContextT, "AcquireToken", new Type[]
			{
				typeof(string),
				typeof(string),
				typeof(Uri),
				this.PromptBehaviorT
			});
			this.AcquireTokenByRefreshTokenM = this.LoadAndValidateMethod(this.AuthenticationContextT, "AcquireTokenByRefreshToken", new Type[]
			{
				typeof(string),
				typeof(string)
			});
			this.AcquireTokenUserIdM = this.LoadAndValidateMethod(this.AuthenticationContextT, "AcquireToken", new Type[]
			{
				typeof(string),
				typeof(string),
				typeof(Uri),
				this.PromptBehaviorT,
				this.UserIdentifierT
			});
			this.AcquireTokenCredentialsM = this.LoadAndValidateMethod(this.AuthenticationContextT, "AcquireToken", new Type[]
			{
				typeof(string),
				typeof(string),
				this.UserCredentialT
			});
			this.TokenCacheP = this.LoadAndValidateProperty(this.AuthenticationContextT, "TokenCache");
			this.AccessTokenP = this.LoadAndValidateProperty(this.AuthenticationResultT, "AccessToken");
			this.RefreshTokenP = this.LoadAndValidateProperty(this.AuthenticationResultT, "RefreshToken");
			this.ExpiresOnP = this.LoadAndValidateProperty(this.AuthenticationResultT, "ExpiresOn");
			this.PromptBehaviorAutoV = this.LoadAndValidateEnumValue(this.PromptBehaviorT, "Auto");
		}

		private void LoadTokenCacheTypes(Assembly assembly)
		{
			this.TokenCacheT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCache");
			this.TokenCacheNotificationT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCacheNotification");
			this.BeforeAccessP = this.LoadAndValidateProperty(this.TokenCacheT, "BeforeAccess");
			this.AfterAccessP = this.LoadAndValidateProperty(this.TokenCacheT, "AfterAccess");
			this.BeforeWriteP = this.LoadAndValidateProperty(this.TokenCacheT, "BeforeWrite");
			this.HasStateChangedP = this.LoadAndValidateProperty(this.TokenCacheT, "HasStateChanged");
			this.SerializeM = this.LoadAndValidateMethod(this.TokenCacheT, "Serialize", new Type[0]);
			this.DeserializeM = this.LoadAndValidateMethod(this.TokenCacheT, "Deserialize", new Type[]
			{
				typeof(byte[])
			});
			this.ReadItemsM = this.LoadAndValidateMethod(this.TokenCacheT, "ReadItems", new Type[0]);
			this.ClearM = this.LoadAndValidateMethod(this.TokenCacheT, "Clear", new Type[0]);
			this.TokenCacheItemT = this.LoadAndValidateType(assembly, "Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCacheItem");
			this.AuthorityP = this.LoadAndValidateProperty(this.TokenCacheItemT, "Authority");
			this.ClientIdP = this.LoadAndValidateProperty(this.TokenCacheItemT, "ClientId");
			this.DisplayableIdP = this.LoadAndValidateProperty(this.TokenCacheItemT, "DisplayableId");
		}

		private Type LoadAndValidateType(Assembly assembly, string typeName)
		{
			Type type = assembly.GetType(typeName);
			if (type == null)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_ClaimsToken_AdalLoadingError(string.Format("{0} type", typeName)));
			}
			return type;
		}

		private PropertyInfo LoadAndValidateProperty(Type type, string propertyName)
		{
			PropertyInfo property = type.GetProperty(propertyName);
			if (property == null)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_ClaimsToken_AdalLoadingError(string.Format("{0}.{1} property", type.Name, propertyName)));
			}
			return property;
		}

		private object LoadAndValidateEnumValue(Type enumType, string enumValueName)
		{
			object obj = Enum.GetValues(enumType).Cast<object>().FirstOrDefault((object val) => val.ToString() == enumValueName);
			if (obj == null)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_ClaimsToken_AdalLoadingError(string.Format("{0}.{1} enum value", enumType.Name, enumValueName)));
			}
			return obj;
		}

		private MethodInfo LoadAndValidateMethod(Type type, string methodName, params Type[] argTypes)
		{
			MethodInfo methodInfo = (argTypes.Length == 0) ? type.GetMethod(methodName) : type.GetMethod(methodName, argTypes);
			if (methodInfo == null)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_ClaimsToken_AdalLoadingError(string.Format("{0}.{1}({2}) method", type.Name, methodName, string.Join(", ", (from t in argTypes
				select t.Name).ToArray<string>()))));
			}
			return methodInfo;
		}

		private static Stream GetResourceAsStream(string resourceName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			string text = manifestResourceNames.FirstOrDefault((string rn) => rn.EndsWith(resourceName));
			if (text == null)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_ClaimsToken_AdalLoadingError(string.Format("{0} from embedded resource", resourceName)));
			}
			return executingAssembly.GetManifestResourceStream(text);
		}
	}
}
