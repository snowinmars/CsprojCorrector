﻿using System;
using System.IO;

namespace ConsoleApplication1
{
	public abstract class CsprojBaseTest : IDisposable
	{
		protected CsprojBaseTest()
		{
			FullPathToCurrectDirectory = Directory.GetCurrentDirectory();
			PathToCsprojFile = Path.Combine(FullPathToCurrectDirectory, Guid.NewGuid().ToString());

			CreateCsprojFile();
		}

		protected string PathToCsprojFile { get; }

		private string FullPathToCurrectDirectory { get; }

		public void Dispose()
		{
			RemoveCsprojFile();
		}

		private void CreateCsprojFile()
		{
			File.WriteAllText(PathToCsprojFile, DefaultCsprojFile);
		}

		#region DefaultCsprojFile

		private const string DefaultCsprojFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""14.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""..\packages\xunit.runner.visualstudio.2.2.0\build\net20\xunit.runner.visualstudio.props"" Condition=""Exists('..\packages\xunit.runner.visualstudio.2.2.0\build\net20\xunit.runner.visualstudio.props')"" />
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{EB0CFA87-6C36-4A7B-A6DA-E26C5321F182}</ProjectGuid>
    <OutputType>Exe</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>ConsoleApplication1</RootNamespace>
    <AssemblyName>ConsoleApplication1</AssemblyName>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <NuGetPackageImportStamp>
    </NuGetPackageImportStamp>
    <LangVersion>default</LangVersion>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <LangVersion>5</LangVersion>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <LangVersion>4</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.Xml.Linq"" />
    <Reference Include=""System.Data.DataSetExtensions"" />
    <Reference Include=""Microsoft.CSharp"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Net.Http"" />
    <Reference Include=""System.Xml"" />
    <Reference Include=""xunit.abstractions, Version=2.0.0.0, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c, processorArchitecture=MSIL"">
      <HintPath>..\packages\xunit.abstractions.2.0.1\lib\net35\xunit.abstractions.dll</HintPath>
      <Private>True</Private>
    </Reference>
    <Reference Include=""xunit.assert, Version=2.2.0.3545, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c, processorArchitecture=MSIL"">
      <HintPath>..\packages\xunit.assert.2.2.0\lib\netstandard1.1\xunit.assert.dll</HintPath>
      <Private>True</Private>
    </Reference>
    <Reference Include=""xunit.core, Version=2.2.0.3545, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c, processorArchitecture=MSIL"">
      <HintPath>..\packages\xunit.extensibility.core.2.2.0\lib\netstandard1.1\xunit.core.dll</HintPath>
      <Private>True</Private>
    </Reference>
    <Reference Include=""xunit.execution.desktop, Version=2.2.0.3545, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c, processorArchitecture=MSIL"">
      <HintPath>..\packages\xunit.extensibility.execution.2.2.0\lib\net452\xunit.execution.desktop.dll</HintPath>
      <Private>True</Private>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""CsprojConfigState.cs"" />
    <Compile Include=""CsprojCorrector.cs"" />
    <Compile Include=""CsprojCorrectorTests.cs"" />
    <Compile Include=""Program.cs"" />
    <Compile Include=""Properties\AssemblyInfo.cs"" />
    <Compile Include=""CsprojBaseTest.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""App.config"" />
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
  <Target Name=""EnsureNuGetPackageBuildImports"" BeforeTargets=""PrepareForBuild"">
    <PropertyGroup>
      <ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>
    </PropertyGroup>
    <Error Condition=""!Exists('..\packages\xunit.runner.visualstudio.2.2.0\build\net20\xunit.runner.visualstudio.props')"" Text=""$([System.String]::Format('$(ErrorText)', '..\packages\xunit.runner.visualstudio.2.2.0\build\net20\xunit.runner.visualstudio.props'))"" />
  </Target>
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name=""BeforeBuild"">
  </Target>
  <Target Name=""AfterBuild"">
  </Target>
  -->
</Project>
";

		#endregion DefaultCsprojFile

		private void RemoveCsprojFile()
		{
			File.Delete(this.PathToCsprojFile);
		}
	}
}