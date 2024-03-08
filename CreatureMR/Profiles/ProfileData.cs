namespace CackleCrew.ThisIsMagical
{
    //Allows us to control how the data is handled//
    public enum ProfileDataType
    {
        Generic,
        Boolean,
        Color,
        Character,
        ShaderOption
    }
    //Profile Data!!
    //This is part of the profile update,
    //This allows us to fine-tune how data is stored and handled//
    public class ProfileData
    {
        private string _data;
        private ProfileDataType _type;
        public string Data
        {
            get => _data;
            set => _data = value;
        }
        public ProfileDataType Type
        {
            get => _type;
            set => _type = value;
        }
        public int Size
        {
            get { return DataSize(); }
        }
        public int DataSize()
        {
            switch (_type)
            {
                case ProfileDataType.Boolean:
                    return 1;
                case ProfileDataType.Color:
                    return 6;
                case ProfileDataType.Character:
                    return 1;
                case ProfileDataType.ShaderOption:
                    return 1;
                default:
                    return _data.Length;
            }
        }
        //Encoding Happens When Retreiving Data.
        public string EncodeData()
        {
            switch (_type)
            {
                case ProfileDataType.Boolean:
                    return EncodeBooleanData(_data);
                case ProfileDataType.Color:
                    return EncodeColorData(_data);
                case ProfileDataType.Character:
                    return EncodeCharacterData(_data);
                case ProfileDataType.ShaderOption:
                    return EncodeShaderData(_data);
                default:
                    return _data;
            }
        }
        //Decoding Happens When Storing Data.
        public string DecodeData(string value)
        {
            switch (_type)
            {
                case ProfileDataType.Boolean:
                    _data = DecodeBooleanData(value);
                    break;
                case ProfileDataType.Color:
                    _data = DecodeColorData(value);
                    break;
                case ProfileDataType.Character:
                    _data = DecodeCharacterData(value);
                    break;
                case ProfileDataType.ShaderOption:
                    _data = DecodeShaderData(value);
                    break;
                default:
                    _data = value;
                    break;
            }
            return _data;
        }
        private string EncodeBooleanData(string value)
        {
            switch (value)
            {
                case "1":
                    return "TRUE";
                default:
                    return "FALSE";
            }
        }
        private string DecodeBooleanData(string value)
        {
            switch (value.ToUpper())
            {
                case "TRUE":
                    return "1";
                default:
                    return "0";
            }
        }
        private string EncodeColorData(string value)
        {
            return string.IsNullOrEmpty(value) ? value : $"#{value}";
        }
        private string DecodeColorData(string value)
        {
            if (value.StartsWith("#"))
                return value.Substring(value.LastIndexOf('#')+1);
            return value;
        }
        private string EncodeCharacterData(string value)
        {
            return value.Substring(0, 1);
        }
        private string DecodeCharacterData(string value)
        {
            return EncodeCharacterData(value);
        }
        private string EncodeShaderData(string value)
        {
            switch (value)
            {
                case "1":
                    return "A";
                case "2":
                    return "B";
                case "3":
                    return "C";
                case "4":
                    return "D";
                default:
                    return "None";
            }
        }
        private string DecodeShaderData(string value)
        {
            switch (value)
            {
                case "A":
                    return "1";
                case "B":
                    return "2";
                case "C":
                    return "3";
                case "D":
                    return "4";
                default:
                    return "0";
            }
        }
    }
}
