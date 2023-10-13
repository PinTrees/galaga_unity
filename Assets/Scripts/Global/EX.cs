using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EX
{
    static readonly string[] CurrencyUnits = new string[] { 
        "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", 
        "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", 
        "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL",
        "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
        "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN",
        "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", 
        "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", 
        "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", };

    /// <summary>
    /// double �� �����͸� Ŭ��Ŀ ������ ȭ�� ������ ǥ��
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string ToCurrencyString(this double number)
    {
        string zero = "0";

        if (-1d < number && number < 1d)
        {
            return zero;
        }

        if (double.IsInfinity(number))
        {
            return "Infinity";
        }

        //  ��ȣ ��� ���ڿ�
        string significant = (number < 0) ? "-" : string.Empty;

        //  ������ ����
        string showNumber = string.Empty;

        //  ���� ���ڿ�
        string unityString = string.Empty;

        //  ������ �ܼ�ȭ ��Ű�� ���� ������ ���� ǥ�������� ������ �� ó��
        string[] partsSplit = number.ToString("E").Split('+');

        //  ����
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  ���� (�ڸ��� ǥ��)
        if (!int.TryParse(partsSplit[1], out int exponent))
        {
            Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  ���� ���ڿ� �ε���
        int quotient = exponent / 3;

        //  �������� ������ �ڸ��� ��꿡 ���(10�� �ŵ������� ���)
        int remainder = exponent % 3;

        //  1A �̸��� �׳� ǥ��
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString(); 
        }
        else
        {
            //  10�� �ŵ������� ���ؼ� �ڸ��� ǥ������ ����� �ش�.
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

            //  �Ҽ� ��°�ڸ������� ����Ѵ�.
            showNumber = temp.ToString("F").Replace(".00", "");
        }

        unityString = CurrencyUnits[quotient];

        return string.Format("{0}{1}{2}", significant, showNumber, unityString);
    }


    public static void ForEach<T>(this T[] arr, Action<T> func)
    {
        if (null == arr)
            return;

        for(int i = 0; i < arr.Length; ++i)
        {
            func(arr[i]);
        }
    }

    public static void ForEach<K, T>(this Dictionary<K, T> arr, Action<K, T> func)
    {
        if (null == arr)
            return;

        for (int i = 0; i < arr.Keys.Count; ++i)
        {
            var key = arr.Keys.ElementAt(i);
            var value = arr.Values.ElementAt(i);

            func(key, value);
        }
    }
}
