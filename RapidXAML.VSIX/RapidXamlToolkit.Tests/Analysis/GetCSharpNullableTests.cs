﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Analysis
{
    [TestClass]
    public class GetCSharpNullableTests : CSharpTestsBase
    {
        private Profile NullableTestsProfile
        {
            get
            {
                var profile = TestProfile.CreateEmpty();
                profile.Mappings.Add(new Mapping
                {
                    Type = "bool",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<Bool />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "bool?",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<BoolQ />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "Nullable<bool>",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<NullBool />",
                });

                // This should never match as the one above will be found first
                profile.Mappings.Add(new Mapping
                {
                    Type = "System.Nullable<bool>",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<SysNullBool />",
                });

                return profile;
            }
        }

        [TestMethod]
        public void GetNullablePropertiesInClass()
        {
            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Cla*ss1
    {
        public bool MyBool { get; set; }
        public bool? MyBoolQ { get; set; }
        public Nullable<bool> MyNullableBool { get; set; }
        public System.Nullable<bool> MyFqNullableBool { get; set; }
    }
}";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = @"<Bool />
<BoolQ />
<NullBool />
<NullBool />
",
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyShorthand()
        {
            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Class1
    {
        public bool? MyBoo*lQ { get; set; }
    }
}";

            var expected = new AnalyzerOutput
            {
                Name = "MyBoolQ",
                Output = @"<BoolQ />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyLonghand()
        {
            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Class1
    {
        public Nullable<bool> MyNull*ableBool { get; set; }
    }
}";

            var expected = new AnalyzerOutput
            {
                Name = "MyNullableBool",
                Output = @"<NullBool />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyFullyQualified()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public System.Nullable<bool> MyFqNullab*leBool { get; set; }
    }
}";

            var expected = new AnalyzerOutput
            {
                Name = "MyFqNullableBool",
                Output = @"<NullBool />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetListOfNullableProperty()
        {
            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Class1
    {
        public List<bool?> MyListOfNu*llables { get; set; }
    }
}";

            var profile = this.NullableTestsProfile;
            profile.Mappings.Add(new Mapping
            {
                Type = "List<bool?>",
                NameContains = string.Empty,
                IfReadOnly = false,
                Output = "<LB? />",
            });

            var expected = new AnalyzerOutput
            {
                Name = "MyListOfNullables",
                Output = @"<LB? />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetNullablePropertiesInSelection()
        {
            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Class1
    {
       * public bool MyBool { get; set; }
        public bool? MyBoolQ { get; set; }
        public Nullable<bool> MyNullableBool { get; set; }
        public System.Nullable<bool> MyFqNullableBool { get; set; }*
    }
}";

            var expected = new AnalyzerOutput
            {
                Name = "MyBool, MyBoolQ and 2 other properties",
                Output = @"<Bool />
<BoolQ />
<NullBool />
<NullBool />",
                OutputType = AnalyzerOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }
    }
}
