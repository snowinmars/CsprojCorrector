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
					XmlElement langVersionNode = GetLangVersionNode(version);
					element.AppendChild(langVersionNode);
				}
				else // if there is - change the version
				{
					node.InnerText = version;
				}
			}
		}

		private XmlElement GetLangVersionNode(string version)
		{
			// if there is namespace - use it, if no - try to use something
			string namespaceUri = _csprojXmlDocument.DocumentElement?.NamespaceURI ?? Guid.NewGuid().ToString();

			XmlElement element = _csprojXmlDocument.CreateElement(Const.LangVersionTagName, namespaceUri);
			element.InnerText = version;

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
			public const string ConditionTagAttributeName = "Condition";
			public const string DefaultLangVersion = "default";
			public const string LangVersionTagName = "LangVersion";
			public const string PropertyGroupDebugAttributeValue = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
			public const string PropertyGroupReleaseAttributeValue = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
			public const string PropertyGroupTagName = "PropertyGroup";
		}
	}
}