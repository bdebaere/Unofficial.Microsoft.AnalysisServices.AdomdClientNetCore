using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SSPIWrapper
	{
		private SSPIWrapper()
		{
		}

		internal static SecurityPackageInfoClass[] EnumerateSecurityPackages()
		{
			int num = 0;
			IntPtr zero = IntPtr.Zero;
			int num2 = UnsafeNclNativeMethods.NativeNTSSPI.EnumerateSecurityPackagesW(out num, out zero);
			if (num2 != 0)
			{
				throw new Win32Exception(num2);
			}
			SecurityPackageInfoClass[] result;
			try
			{
				SecurityPackageInfoClass[] array = new SecurityPackageInfoClass[num];
				IntPtr intPtr = zero;
				for (int i = 0; i < num; i++)
				{
					array[i] = new SecurityPackageInfoClass(intPtr);
					intPtr = IntPtrHelper.Add(intPtr, SecurityPackageInfo.Size);
				}
				result = array;
			}
			finally
			{
				UnsafeNclNativeMethods.NativeNTSSPI.FreeContextBuffer(zero);
			}
			return result;
		}

		internal static int InitializeSecurityContext(ref SecurityHandle credential, ref SecurityHandle context, string targetName, int requirements, Endianness datarep, SecurityBufferClass inputBuffer, ref SecurityHandle newContext, SecurityBufferClass outputBuffer, ref int attributes, ref long timestamp)
		{
			SecurityBufferClass[] inputBuffers = null;
			SecurityBufferClass[] array = null;
			if (inputBuffer != null)
			{
				inputBuffers = new SecurityBufferClass[]
				{
					inputBuffer
				};
			}
			if (outputBuffer != null)
			{
				array = new SecurityBufferClass[]
				{
					outputBuffer
				};
			}
			int result = SSPIWrapper.InitializeSecurityContext(ref credential, ref context, targetName, requirements, datarep, inputBuffers, ref newContext, array, ref attributes, ref timestamp);
			outputBuffer.type = array[0].type;
			outputBuffer.size = array[0].size;
			outputBuffer.token = array[0].token;
			return result;
		}

		private static int InitializeSecurityContext(ref SecurityHandle credential, ref SecurityHandle context, string targetName, int requirements, Endianness datarep, SecurityBufferClass[] inputBuffers, ref SecurityHandle newContext, SecurityBufferClass[] outputBuffers, ref int attributes, ref long timestamp)
		{
			GCHandle[] array = null;
			GCHandle[] array2 = null;
			int num = 0;
			try
			{
				if (outputBuffers != null)
				{
					array = SSPIWrapper.PinBuffers(outputBuffers);
				}
				SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(outputBuffers);
				try
				{
					if (inputBuffers == null)
					{
						num = UnsafeNclNativeMethods.NativeNTSSPI.InitializeSecurityContextW(ref credential, IntPtr.Zero, targetName, requirements, 0, (int)datarep, IntPtr.Zero, 0, ref newContext, ref securityBufferDescriptor, ref attributes, ref timestamp);
					}
					else
					{
						array2 = SSPIWrapper.PinBuffers(inputBuffers);
						SecurityBufferDescriptor securityBufferDescriptor2 = new SecurityBufferDescriptor(inputBuffers);
						try
						{
							num = UnsafeNclNativeMethods.NativeNTSSPI.InitializeSecurityContextW(ref credential, ref context, targetName, requirements, 0, (int)datarep, ref securityBufferDescriptor2, 0, ref newContext, ref securityBufferDescriptor, ref attributes, ref timestamp);
						}
						finally
						{
							securityBufferDescriptor2.FreeAllBuffers(0);
						}
					}
					if (num == 0 || num == 590610)
					{
						SecurityBufferClass[] array3 = securityBufferDescriptor.marshall();
						for (int i = 0; i < outputBuffers.Length; i++)
						{
							outputBuffers[i] = array3[i];
						}
					}
				}
				finally
				{
					securityBufferDescriptor.FreeAllBuffers(requirements);
				}
			}
			finally
			{
				if (array != null)
				{
					SSPIWrapper.FreeGCHandles(array);
				}
				if (array2 != null)
				{
					SSPIWrapper.FreeGCHandles(array2);
				}
			}
			return num;
		}

		public static int EncryptMessage(ref SecurityHandle context, SecurityBufferClass[] input, int sequenceNumber)
		{
			GCHandle[] gcHandles = SSPIWrapper.PinBuffers(input);
			int result;
			try
			{
				SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(input);
				try
				{
					int num = UnsafeNclNativeMethods.NativeNTSSPI.EncryptMessage(ref context, 0, ref securityBufferDescriptor, sequenceNumber);
					SecurityBufferClass[] array = securityBufferDescriptor.marshall();
					for (int i = 0; i < input.Length; i++)
					{
						input[i] = array[i];
					}
					result = num;
				}
				finally
				{
					securityBufferDescriptor.FreeAllBuffers(0);
				}
			}
			finally
			{
				SSPIWrapper.FreeGCHandles(gcHandles);
			}
			return result;
		}

		public static int DecryptMessage(ref SecurityHandle context, SecurityBufferClass[] input, int sequenceNumber)
		{
			int num = 0;
			GCHandle[] gcHandles = SSPIWrapper.PinBuffers(input);
			int result;
			try
			{
				SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(input);
				try
				{
					int num2 = UnsafeNclNativeMethods.NativeNTSSPI.DecryptMessage(ref context, ref securityBufferDescriptor, sequenceNumber, ref num);
					if (num2 == 0)
					{
						SecurityBufferClass[] array = securityBufferDescriptor.marshall();
						for (int i = 0; i < input.Length; i++)
						{
							input[i] = array[i];
						}
					}
					result = num2;
				}
				finally
				{
					securityBufferDescriptor.FreeAllBuffers(0);
				}
			}
			finally
			{
				SSPIWrapper.FreeGCHandles(gcHandles);
			}
			return result;
		}

		public static object QueryContextAttributes(SecurityContext securityContext, ContextAttribute contextAttribute)
		{
			int cb;
			if (contextAttribute != ContextAttribute.Sizes)
			{
				if (contextAttribute != ContextAttribute.StreamSizes)
				{
					throw new NotImplementedException();
				}
				cb = 20;
			}
			else
			{
				cb = 16;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(cb);
			object result;
			try
			{
				int num = UnsafeNclNativeMethods.NativeNTSSPI.QueryContextAttributes(ref securityContext.Handle, (int)contextAttribute, intPtr);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
				object obj = null;
				if (contextAttribute != ContextAttribute.Sizes)
				{
					if (contextAttribute == ContextAttribute.StreamSizes)
					{
						obj = new StreamSizes(intPtr);
					}
				}
				else
				{
					obj = new Sizes(intPtr);
				}
				result = obj;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		internal static GCHandle[] PinBuffers(SecurityBufferClass[] securityBuffers)
		{
			GCHandle[] array = new GCHandle[securityBuffers.Length];
			for (int i = 0; i < securityBuffers.Length; i++)
			{
				if (securityBuffers[i] != null && securityBuffers[i].token != null)
				{
					array[i] = GCHandle.Alloc(securityBuffers[i].token, GCHandleType.Pinned);
				}
			}
			return array;
		}

		internal static void FreeGCHandles(GCHandle[] gcHandles)
		{
			if (gcHandles == null)
			{
				return;
			}
			for (int i = 0; i < gcHandles.Length; i++)
			{
				if (gcHandles[i].IsAllocated)
				{
					gcHandles[i].Free();
				}
			}
		}

		public static int MakeSignature(ref SecurityHandle context, int QOP, SecurityBufferClass[] input, int sequenceNumber)
		{
			GCHandle[] gcHandles = SSPIWrapper.PinBuffers(input);
			int result;
			try
			{
				SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(input);
				try
				{
					int num = UnsafeNclNativeMethods.NativeNTSSPI.MakeSignature(ref context, QOP, ref securityBufferDescriptor, sequenceNumber);
					SecurityBufferClass[] array = securityBufferDescriptor.marshall();
					for (int i = 0; i < input.Length; i++)
					{
						input[i] = array[i];
					}
					result = num;
				}
				finally
				{
					securityBufferDescriptor.FreeAllBuffers(0);
				}
			}
			finally
			{
				SSPIWrapper.FreeGCHandles(gcHandles);
			}
			return result;
		}

		public static int VerifySignature(ref SecurityHandle context, int QOP, SecurityBufferClass[] input, int sequenceNumber)
		{
			GCHandle[] gcHandles = SSPIWrapper.PinBuffers(input);
			int result;
			try
			{
				SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(input);
				try
				{
					int num = UnsafeNclNativeMethods.NativeNTSSPI.VerifySignature(ref context, ref securityBufferDescriptor, sequenceNumber, QOP);
					SecurityBufferClass[] array = securityBufferDescriptor.marshall();
					for (int i = 0; i < input.Length; i++)
					{
						input[i] = array[i];
					}
					result = num;
				}
				finally
				{
					securityBufferDescriptor.FreeAllBuffers(0);
				}
			}
			finally
			{
				SSPIWrapper.FreeGCHandles(gcHandles);
			}
			return result;
		}
	}
}
