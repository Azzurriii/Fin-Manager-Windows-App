using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Model
{
    public class Account : INotifyPropertyChanged
    {
        [JsonPropertyName("account_id")]
        public int AccountId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        private string? _accountName;

        [JsonPropertyName("account_name")]// Make this nullable
        public string AccountName
        {
            get { return _accountName ?? string.Empty; } // Return an empty string if null
            set
            {
                if (_accountName != value)
                {
                    _accountName = value;
                    OnPropertyChanged(nameof(AccountName));
                }
            }
        }

        private string? _accountType;

        [JsonPropertyName("account_type")]
        public string AccountType
        {
            get { return _accountType ?? string.Empty; }
            set
            {
                if (_accountType != value)
                {
                    _accountType = value;
                    OnPropertyChanged(nameof(AccountType));
                }
            }
        }

        private decimal _initialBalance;

        [JsonPropertyName("initial_balance")]
        public decimal InitialBalance
        {
            get { return _initialBalance; }
            set
            {
                if (_initialBalance != value)
                {
                    _initialBalance = value;
                    OnPropertyChanged(nameof(InitialBalance));
                }
            }
        }

        private decimal _currentBalance;

        [JsonPropertyName("current_balance")]
        public decimal CurrentBalance
        {
            get { return _currentBalance; }
            set
            {
                if (_currentBalance != value)
                {
                    _currentBalance = value;
                    OnPropertyChanged(nameof(CurrentBalance));
                }
            }
        }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty; // Initialize to empty string to avoid null

        [JsonPropertyName("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [JsonPropertyName("update_at")]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged; // Make this nullable
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
