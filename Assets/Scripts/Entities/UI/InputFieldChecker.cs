using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI
{
    public class InputFieldChecker : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextAsset _textAsset;
        
        private const RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);
        
        private static readonly Regex EnglishRegex = new Regex(
            @"\b(f[\W_]*u[\W_]*c[\W_]*k|s[\W_]*h[\W_]*i[\W_]*t|b[\W_]*i[\W_]*t[\W_]*c[\W_]*h|c[\W_]*u[\W_]*n[\W_]*t|d[\W_]*i[\W_]*c[\W_]*k|a[\W_]*s[\W_]*s[\W_]*h|n[\W_]*i[\W_]*g[\W_]*g)\b", 
            _options, Timeout);
        
        private static readonly Regex RussianRegex = new Regex(@"\b(хуй|п[\W_]*и[\W_]*з[\W_]*д|(?<!хл|тр|погр)еб(?!у)|б[\W_]*л[\W_]*я(?![а-я])|м[\W_]*у[\W_]*д[\W_]*[аио]|г[\W_]*о[\W_]*н[\W_]*д[\W_]*о[\W_]*н)\b", 
            _options, Timeout);
        
        public static bool ContainsProfanity(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;
            
            string normalized = NormalizeInput(input);
            
            if (EnglishRegex.IsMatch(normalized) || RussianRegex.IsMatch(normalized)) return true;

            return false;
        }

        private static string NormalizeInput(string input)
        {
            char[] chars = input.ToLowerInvariant().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                
                if (c == '0') c = 'o';
                else if (c == '1') c = 'i';
                else if (c == '3') c = 'е';
                else if (c == '4') c = 'a';
                else if (c == '5') c = 's';
                else if (c == '7') c = 't';
                else if (c == '@') c = 'a';
                else if (c == '$') c = 's';
            
                switch (c)
                {
                    case 'a': chars[i] = 'а'; break;
                    case 'e': chars[i] = 'е'; break;
                    case 'y': chars[i] = 'у'; break;
                    case 'p': chars[i] = 'р'; break;
                    case 'x': chars[i] = 'х'; break;
                    case 'c': chars[i] = 'с'; break;
                    case 'o': chars[i] = 'о'; break;
                }
            }
            return new string(chars);
        }
        
        public void OnValueChanged(string value)
        {
            if (ContainsProfanity(value.ToLower()) || _textAsset.text.Contains(value.ToLower()))
            {
                _button.interactable = false;
            }
            else
            {
                _button.interactable = true;
            }
        }
    }
}