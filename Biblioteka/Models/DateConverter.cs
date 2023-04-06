using System;
using System.Linq;


namespace Biblioteka.Models
{
    class DateConverter
    {
        static public string GetDateOnly(string dateTime)
        {
            string[] splitted;
            if (dateTime.Contains(' '))
            {
                splitted = dateTime.Split(' ');
                return splitted[0];
            }
            else
            {
                return dateTime;
            }
        }

        static public string MakeDatePickerDate(string DateTime)
        {
            //27.03.2023 05:17:40  DateTime


            string DatePickerDate;
            string[] splittedDateTime = DateTime.Split(' ');
            DatePickerDate = splittedDateTime[0];
            DatePickerDate = DatePickerDate.Replace('.', '/');
            string[] splitedDate = DatePickerDate.Split('/');
            DatePickerDate = splitedDate[1] + '/' + splitedDate[0] + '/' + splitedDate[2];
            return DatePickerDate + " 12:00:00 AM";
            // need: "1/29/2000 12:00:00 AM" DatePicker
        }
        static private DateTime DateToDateTime(string DatePicker) //datepicker to dateTime
        {
            string[] splittedDatePicker = DatePicker.Split(' ');
            string justDate = splittedDatePicker[0];
            string[] splittedJustDate = justDate.Split('/');
            string dateTime = splittedJustDate[1] + "." + splittedJustDate[0] + "." + splittedJustDate[2] + " 05:17:40";
            

            return DateTime.Parse(dateTime);
        }
        static public string AddDaysTo(string datePicker,int days)
        {
            DateTime date = DateToDateTime(datePicker);
            double ddays = (double)days;
            var dateDaysAdded =date.AddDays(ddays);


            return MakeDatePickerDate(dateDaysAdded.ToString());
        }

        static public string MakeSQLDateOnly(string datePickerDate)
        {
            if (datePickerDate != null || datePickerDate.Contains(" "))
            {
                string SQLDate;
                string[] splittedDateTime = datePickerDate.Split(' ');
                SQLDate = splittedDateTime[0];
                SQLDate = SQLDate.Replace('/', '-');
                string[] splitedDate = SQLDate.Split('-');
                SQLDate = splitedDate[2] + '-' + splitedDate[0] + '-' + splitedDate[1];
                return SQLDate;

            }
            return datePickerDate;

        }






    }
}
