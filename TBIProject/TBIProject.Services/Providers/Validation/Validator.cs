using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TBIProject.Services.Providers.Validation
{
    public class Validator : IValidator
    {
        public async Task<bool> ValidateEGN(string egn)
        {
            if (string.IsNullOrEmpty(egn)) return false;

            if (egn.Length != 10)
                return false;
            var result = Regex.Match(egn, "[^0 - 9]");
            if (!result.Success)
                return false;
            var year = int.Parse(egn.Substring(0, 2));
            var month = int.Parse(egn.Substring(2, 2));
            var day = int.Parse(egn.Substring(4, 2));

            if (month >= 40)
            {
                year += 2000;
                month -= 40;
            }
            else if (month >= 20)
            {
                year += 1800;
                month -= 20;
            }
            else
            {
                year += 1900;
            }

            if (!isValidDate(year, month, day))
                return false;

            var checkSum = 0;
            var weights = new[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };

            for (int i = 0; i < weights.Length; ++i)
            {
                checkSum += weights[i] * int.Parse(egn[i].ToString());
            }

            checkSum %= 11;
            checkSum %= 10;

            if (checkSum != int.Parse(egn[9].ToString()))
                return false;

            return true;

        }

        public async Task<bool> ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            var result = Regex.Match(phone, "^((359)|(\\+359)|(0)){1}[\\d]{9,11}$");
            if (result.Success)
            {
                return true;
            }
            return false;

        }
        private bool isValidDate(int y, int m, int d)
        {
            var date = new DateTime(y, m - 1, d);
            return (date.Month + 1) == m && date.Day == d;
        }

    }

}
