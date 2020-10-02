using System;
using System.Runtime.CompilerServices;

namespace DevilDaggersAssetEditor.New.Utils
{
	public static class ExceptionUtils
	{
		public static Exception EnumNotImplemented<TEnum>(TEnum unimplementedValue, [CallerMemberName] string methodName = "")
			where TEnum : Enum
			=> new NotImplementedException($"{nameof(TEnum)} '{unimplementedValue}' has not been implemented in the '{methodName}' method.");
	}
}