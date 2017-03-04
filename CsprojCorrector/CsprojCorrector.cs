using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ConsoleApplication1
{
	public class CsprojCorrector : IDisposable
	{
		private readonly XmlDocument _csprojXmlDocument;

		private IDictionary<string, string> CodeContractCorrectValues = new Dictionary<string, string>
		{
			{Const.CodeContractsEnableRuntimeChecking, "True"},
			{Const.CodeContractsRuntimeOnlyPublicSurface, "False"},
			{Const.CodeContractsRuntimeThrowOnFailure, "True"},
			{Const.CodeContractsRuntimeCallSiteRequires, "False"},
			{Const.CodeContractsRuntimeSkipQuantifiers, "False"},
			{Const.CodeContractsRunCodeAnalysis, "True"},
			{Const.CodeContractsNonNullObligations, "True"},
			{Const.CodeContractsBoundsObligations, "True"},
			{Const.CodeContractsArithmeticObligations, "True"},
			{Const.CodeContractsEnumObligations, "True"},
			{Const.CodeContractsRedundantAssumptions, "True"},
			{Const.CodeContractsAssertsToContractsCheckBox, "True"},
			{Const.CodeContractsRedundantTests, "True"},
			{Const.CodeContractsMissingPublicRequiresAsWarnings, "True"},
			{Const.CodeContractsMissingPublicEnsuresAsWarnings, "True"},
			{Const.CodeContractsInferRequires, "True"},
			{Const.CodeContractsInferEnsures, "True"},
			{Const.CodeContractsInferEnsuresAutoProperties, "True"},
			{Const.CodeContractsInferObjectInvariants, "True"},
			{Const.CodeContractsSuggestAssumptions, "True"},
			{Const.CodeContractsSuggestAssumptionsForCallees, "True"},
			{Const.CodeContractsSuggestRequires, "True"},
			{Const.CodeContractsNecessaryEnsures, "True"},
			{Const.CodeContractsSuggestObjectInvariants, "False"},
			{Const.CodeContractsSuggestReadonly, "True"},
			{Const.CodeContractsRunInBackground, "True"},
			{Const.CodeContractsShowSquigglies, "True"},
			{Const.CodeContractsUseBaseLine, "False"},
			{Const.CodeContractsEmitXmlDocs, "True"},
			{Const.CodeContractsCustomRewriterAssembly, ""},
			{Const.CodeContractsCustomRewriterClass, ""},
			{Const.CodeContractsLibPaths, ""},
			{Const.CodeContractsExtraRewriteOptions, ""},
			{Const.CodeContractsExtraAnalysisOptions, ""},
			{Const.CodeContractsSqlServerOption, ""},
			{Const.CodeContractsBaseLineFile, ""},
			{Const.CodeContractsCacheAnalysisResults, "True"},
			{Const.CodeContractsSkipAnalysisIfCannotConnectToCache, "False"},
			{Const.CodeContractsFailBuildOnWarnings, "False"},
			{Const.CodeContractsBeingOptimisticOnExternal, "True"},
			{Const.CodeContractsRuntimeCheckingLevel, "Full"},
			{Const.CodeContractsReferenceAssembly, "Build"},
			{Const.CodeContractsAnalysisWarningLevel, "0"},
		};

		public CsprojCorrector(string csprojFullPath)
		{
			CsprojFullPath = csprojFullPath;
			_csprojXmlDocument = new XmlDocument();
			ConfigState = CsprojConfigState.Debug;

			using (FileStream stream = File.Open(CsprojFullPath, FileMode.Open, FileAccess.ReadWrite))
			{
				_csprojXmlDocument.Load(stream);
			}
		}

		public CsprojConfigState ConfigState { get; set; }
		public string CsprojFullPath { get; }

		public void RemoveAllCodeContractSettings()
		{
			var propertyGroupTagCollection = _csprojXmlDocument.GetElementsByTagName(Const.PropertyGroupTagName)
																.Cast<XmlElement>()
																.Where(e => HandleAttributes(e.Attributes))
																.ToArray();

			if (!propertyGroupTagCollection.Any())
			{
				throw new InvalidOperationException("No PropertyGroup tag with debug attribute found");
			}

			foreach (XmlElement element in propertyGroupTagCollection)
			{
				foreach (var codeContractElement in CodeContractCorrectValues)
				{
					string tagName = codeContractElement.Key;

					XmlNode node = element.ChildNodes
										.Cast<XmlNode>()
										.FirstOrDefault(n => n.Name == tagName);

					if (node != null)
					{
						element.RemoveChild(node);
					}
				}
			}
		

	}

	public void ConfigurateForCodeContractUsing()
		{
			var propertyGroupTagCollection = _csprojXmlDocument.GetElementsByTagName(Const.PropertyGroupTagName)
																.Cast<XmlElement>()
																.Where(e => HandleAttributes(e.Attributes))
																.ToArray();

			if (!propertyGroupTagCollection.Any())
			{
				throw new InvalidOperationException("No PropertyGroup tag with debug attribute found");
			}

			foreach (XmlElement element in propertyGroupTagCollection)
			{
				foreach (var codeContractElement in CodeContractCorrectValues)
				{
					string tagName = codeContractElement.Key;
					string tagValue = codeContractElement.Value;

					XmlNode node = element.ChildNodes
										.Cast<XmlNode>()
										.FirstOrDefault(n => n.Name == tagName);

					if (node == null) // if there isn't tag with this name - create it
					{
						XmlElement newChild = GetNode(tagName, tagValue);
						element.AppendChild(newChild);
					}
					else // if there is - change the version
					{
						node.InnerText = tagValue;
					}
				}
			}
		}

		public void Dispose()
		{
			_csprojXmlDocument.Save(CsprojFullPath);
		}

		/// <summary>
		/// Return the inner text fron LangVersion tag, if it exist. If the tag doesn't exist or if the tag contains "default" as a inner text, method returns "default"
		/// </summary>
		/// <exception cref="InvalidOperationException">No PropertyGroup tag was found</exception>
		/// <returns></returns>
		public string GetLangVersion()
		{
			IEnumerable<XmlElement> propertyGroupTagCollection = _csprojXmlDocument.GetElementsByTagName(Const.PropertyGroupTagName)
																					.Cast<XmlElement>()
																					.Where(e => HandleAttributes(e.Attributes))
																					.ToArray();

			if (propertyGroupTagCollection.Any())
			{
				foreach (XmlElement element in propertyGroupTagCollection)
				{
					XmlNode xmlNode = element.ChildNodes
												.Cast<XmlNode>()
												.FirstOrDefault(n => n.Name == Const.LangVersionTagName);

					return xmlNode?.InnerText ?? Const.DefaultLangVersion;
				}
			}

			throw new InvalidOperationException("No PropertyGroup tag with debug attribute found");
		}

		/// <summary>
		/// Set the inner text to LangVersion tag, if it exist. If the tag doesn't exist, method will create a new one.
		/// </summary>
		/// <exception cref="InvalidOperationException">No PropertyGroup tag was found</exception>
		/// <param name="version">"5" for C# 5 etc, "default" for default</param>
		public void SetLangVersion(string version)
		{
			var propertyGroupTagCollection = _csprojXmlDocument.GetElementsByTagName(Const.PropertyGroupTagName)
																.Cast<XmlElement>()
																.Where(e => HandleAttributes(e.Attributes))
																.ToArray();

			if (!propertyGroupTagCollection.Any())
			{
				throw new InvalidOperationException("No PropertyGroup tag with debug attribute found");
			}

			foreach (XmlElement element in propertyGroupTagCollection)
			{
				var node = element.ChildNodes
									.Cast<XmlNode>()
									.FirstOrDefault(n => n.Name == Const.LangVersionTagName);

				if (node == null) // if there is no LangVersion tag - create it
				{
					XmlElement langVersionNode = GetNode(Const.LangVersionTagName, version);
					element.AppendChild(langVersionNode);
				}
				else // if there is - change the version
				{
					node.InnerText = version;
				}
			}
		}

		private XmlElement GetNode(string tagName, string value, string @namespace = null)
		{
			if (@namespace == null)
			{
				@namespace = _csprojXmlDocument.DocumentElement?.NamespaceURI ?? Guid.NewGuid().ToString();
			}

			XmlElement element = _csprojXmlDocument.CreateElement(tagName, @namespace);
			element.InnerText = value;

			return element;
		}

		private bool HandleAttributes(XmlAttributeCollection xmlAttributeCollection)
		{
			if (xmlAttributeCollection == null)
			{
				throw new ArgumentException("Xml collection is null");
			}

			switch (ConfigState)
			{
				case CsprojConfigState.All:
					return xmlAttributeCollection.Count == 0;

				case CsprojConfigState.Debug:
					return xmlAttributeCollection.Count == 1 &&
						xmlAttributeCollection[Const.ConditionTagAttributeName]?.Value == Const.PropertyGroupDebugAttributeValue;

				case CsprojConfigState.Release:
					return xmlAttributeCollection.Count == 1 &&
						xmlAttributeCollection[Const.ConditionTagAttributeName]?.Value == Const.PropertyGroupReleaseAttributeValue;

				default:
					throw new ArgumentOutOfRangeException(nameof(ConfigState), ConfigState, $"Enum {nameof(ConfigState)} is out of range");
			}
		}

		private static class Const
		{
			public const string CodeContractsAnalysisWarningLevel = "CodeContractsAnalysisWarningLevel";
			public const string CodeContractsArithmeticObligations = "CodeContractsArithmeticObligations";
			public const string CodeContractsAssertsToContractsCheckBox = "CodeContractsAssertsToContractsCheckBox";
			public const string CodeContractsBaseLineFile = "CodeContractsBaseLineFile";
			public const string CodeContractsBeingOptimisticOnExternal = "CodeContractsBeingOptimisticOnExternal";
			public const string CodeContractsBoundsObligations = "CodeContractsBoundsObligations";
			public const string CodeContractsCacheAnalysisResults = "CodeContractsCacheAnalysisResults";
			public const string CodeContractsCustomRewriterAssembly = "CodeContractsCustomRewriterAssembly";
			public const string CodeContractsCustomRewriterClass = "CodeContractsCustomRewriterClass";
			public const string CodeContractsEmitXmlDocs = "CodeContractsEmitXMLDocs";
			public const string CodeContractsEnableRuntimeChecking = "CodeContractsEnableRuntimeChecking";
			public const string CodeContractsEnumObligations = "CodeContractsEnumObligations";
			public const string CodeContractsExtraAnalysisOptions = "CodeContractsExtraAnalysisOptions";
			public const string CodeContractsExtraRewriteOptions = "CodeContractsExtraRewriteOptions";
			public const string CodeContractsFailBuildOnWarnings = "CodeContractsFailBuildOnWarnings";
			public const string CodeContractsInferEnsures = "CodeContractsInferEnsures";
			public const string CodeContractsInferEnsuresAutoProperties = "CodeContractsInferEnsuresAutoProperties";
			public const string CodeContractsInferObjectInvariants = "CodeContractsInferObjectInvariants";
			public const string CodeContractsInferRequires = "CodeContractsInferRequires";
			public const string CodeContractsLibPaths = "CodeContractsLibPaths";
			public const string CodeContractsMissingPublicEnsuresAsWarnings = "CodeContractsMissingPublicEnsuresAsWarnings";
			public const string CodeContractsMissingPublicRequiresAsWarnings = "CodeContractsMissingPublicRequiresAsWarnings";
			public const string CodeContractsNecessaryEnsures = "CodeContractsNecessaryEnsures";
			public const string CodeContractsNonNullObligations = "CodeContractsNonNullObligations";
			public const string CodeContractsRedundantAssumptions = "CodeContractsRedundantAssumptions";
			public const string CodeContractsRedundantTests = "CodeContractsRedundantTests";
			public const string CodeContractsReferenceAssembly = "CodeContractsReferenceAssembly";
			public const string CodeContractsRunCodeAnalysis = "CodeContractsRunCodeAnalysis";
			public const string CodeContractsRunInBackground = "CodeContractsRunInBackground";
			public const string CodeContractsRuntimeCallSiteRequires = "CodeContractsRuntimeCallSiteRequires";
			public const string CodeContractsRuntimeCheckingLevel = "CodeContractsRuntimeCheckingLevel";
			public const string CodeContractsRuntimeOnlyPublicSurface = "CodeContractsRuntimeOnlyPublicSurface";
			public const string CodeContractsRuntimeSkipQuantifiers = "CodeContractsRuntimeSkipQuantifiers";
			public const string CodeContractsRuntimeThrowOnFailure = "CodeContractsRuntimeThrowOnFailure";
			public const string CodeContractsShowSquigglies = "CodeContractsShowSquigglies";
			public const string CodeContractsSkipAnalysisIfCannotConnectToCache = "CodeContractsSkipAnalysisIfCannotConnectToCache";
			public const string CodeContractsSqlServerOption = "CodeContractsSQLServerOption";
			public const string CodeContractsSuggestAssumptions = "CodeContractsSuggestAssumptions";
			public const string CodeContractsSuggestAssumptionsForCallees = "CodeContractsSuggestAssumptionsForCallees";
			public const string CodeContractsSuggestObjectInvariants = "CodeContractsSuggestObjectInvariants";
			public const string CodeContractsSuggestReadonly = "CodeContractsSuggestReadonly";
			public const string CodeContractsSuggestRequires = "CodeContractsSuggestRequires";
			public const string CodeContractsUseBaseLine = "CodeContractsUseBaseLine";
			public const string ConditionTagAttributeName = "Condition";
			public const string DefaultLangVersion = "default";
			public const string LangVersionTagName = "LangVersion";
			public const string PropertyGroupDebugAttributeValue = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
			public const string PropertyGroupReleaseAttributeValue = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
			public const string PropertyGroupTagName = "PropertyGroup";
		}
	}
}