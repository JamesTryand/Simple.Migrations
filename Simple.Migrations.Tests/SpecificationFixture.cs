using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Simple.Testing.Framework;

namespace Simple.Migrations.Tests
{
    [TestFixture]
    public class SpecificationFixture
    {
        [Test, TestCaseSource("GetSpecificationTestCases")]
        public void Verify(SpecificationToRun spec)
        {
            var runner = new SpecificationRunner();

            RunResult result = runner.RunSpecifciation(spec);
            if (result.Passed)
                return;

            Assert.Fail(Format(result));
        }

        public TestCaseData[] GetSpecificationTestCases()
        {
            IEnumerable<SpecificationToRun> specs = TypeReader.GetSpecificationsIn(GetType());
            return specs.Select(AsTestCaseData).ToArray();
        }

        static TestCaseData AsTestCaseData(SpecificationToRun spec)
        {
            var data = new TestCaseData(spec);
            data.SetName(spec.Specification.GetName());
            return data;
        }

        private static string Format(RunResult result)
        {
            var ret = "\n\nSPECIFICATION: " + (result.SpecificationName ?? result.FoundOnMemberInfo.Name) + " ";

            ret += (result.Passed ? "PASSED" : "FAILED") + " " + result.Message + "\n\n";
            if (result.Thrown != null)
                ret += result.Thrown + "\n\n";

            ret += "\tEXPECTATIONS:\n\t-------------\n";

            foreach (var exp in result.Expectations)
            {
                if (!exp.Passed)
                    ret += "\n\t<<<----------\n";

                ret += "\t" + exp.Text + " " + (exp.Passed ? "PASSED" : "FAILED") + "\n";

                if (!exp.Passed)
                {
                    ret += PadMultiLineText(exp.Exception.Message) + "\n\n";
                    ret += "\t>>>----------\n\n";
                }
            }

            return ret;
        }

        static string PadMultiLineText(string txt)
        {
            string[] lines = txt.Split(new[] { "\n" }, StringSplitOptions.None);
            return lines.Aggregate("", (current, line) => current + ("\t" + line + "\n"));
        }
    }
}
