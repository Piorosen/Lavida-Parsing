using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testsa
{
    public class Problem
    {
        public int Id;
        public string Name;
    }
    public class StatusList
    {
        public int RunId;
        public string Name;
        public int ProblemId;
        public string Reuslt;
        public string Memory;
        public string Time;
        public string Language;
        public string CodeLength;
        public string CodeLink;

        public string SubmitTime;
    }


    public class UserInfo
    {
        public string Id = String.Empty;
        public string SolvedProblem = String.Empty;
        public string ConqeustRate = String.Empty;
        public string AverageAttemptSolve = String.Empty;
        public string Level = String.Empty;

        public List<Problem> SolveList = new List<Problem>();

    }
}
