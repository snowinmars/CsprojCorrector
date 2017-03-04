using System.Collections.Generic;
using Xunit;

namespace ConsoleApplication1
{
	public class CsprojCorrectorTests : CsprojBaseTest
	{
		private readonly IDictionary<CsprojConfigState, string> _stateLangVersionBinding = new Dictionary<CsprojConfigState, string>
			{
				{ CsprojConfigState.All, "default" },
				{ CsprojConfigState.Debug, "5" },
				{ CsprojConfigState.Release, "4" },
			};

		[Theory]
		[InlineData(CsprojConfigState.All)]
		[InlineData(CsprojConfigState.Debug)]
		[InlineData(CsprojConfigState.Release)]
		public void CsprojCorrector_GetLangVersion(CsprojConfigState configState)
		{
			CsprojCorrector csprojCorrector = new CsprojCorrector(this.PathToCsprojFile)
			{
				ConfigState = configState
			};

			var actualLangVersion = csprojCorrector.GetLangVersion();

			Assert.Equal(expected: _stateLangVersionBinding[configState], actual: actualLangVersion);
		}

		[Theory]
		[InlineData(CsprojConfigState.All, "default")]
		[InlineData(CsprojConfigState.All, "3")]
		[InlineData(CsprojConfigState.All, "5")]
		[InlineData(CsprojConfigState.Debug, "default")]
		[InlineData(CsprojConfigState.Debug, "3")]
		[InlineData(CsprojConfigState.Debug, "5")]
		[InlineData(CsprojConfigState.Release, "default")]
		[InlineData(CsprojConfigState.Release, "3")]
		[InlineData(CsprojConfigState.Release, "5")]
		public void CsprojCorrector_SetLangVersion(CsprojConfigState configState, string langVersion)
		{
			CsprojCorrector csprojCorrector = new CsprojCorrector(this.PathToCsprojFile)
			{
				ConfigState = configState
			};

			csprojCorrector.SetLangVersion(langVersion);

			string actualLangVersion = csprojCorrector.GetLangVersion();

			Assert.Equal(expected: langVersion, actual: actualLangVersion);
		}
	}
}