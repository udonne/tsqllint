using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Rules.RuleViolations;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public static class RulesTestHelper
    {
        public static void RunRulesTest(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            // arrange
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"..\..\UnitTests\Rules\{rule}\test-files\{testFileName}.sql"));
            var sqlString = File.ReadAllText(path);

            var fragmentVisitor = new SqlRuleVisitor(null, null);
            var ruleViolations = new List<RuleViolation>();

            // todo fix
            var violations = ruleViolations;

            void ErrorCallback(string ruleName, string ruleText, int startLine, int startColumn)
            {
                violations.Add(new RuleViolation(ruleName, startLine, startColumn));
            }

            var visitor = (TSqlFragmentVisitor)Activator.CreateInstance(ruleType, args: (Action<string, string, int, int>)ErrorCallback);
            var textReader = Lib.Utility.ParsingUtility.CreateTextReaderFromString(sqlString);

            var compareer = new RuleViolationComparer();

            // act
            fragmentVisitor.VisitRule(textReader, visitor);

            ruleViolations = ruleViolations.OrderBy(o => o.Line).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();

            // assert
            CollectionAssert.AreEqual(expectedRuleViolations, ruleViolations, compareer);
            Assert.AreEqual(expectedRuleViolations.Count, ruleViolations.Count);
        }
    }
}
