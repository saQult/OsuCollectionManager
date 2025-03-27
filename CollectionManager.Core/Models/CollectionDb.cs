using System.Text;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace CollectionManager.Core.Models
{
    public class CollectionDb()
    {
        private List<Collection> _collections = [];
        public List<Collection> Collections { get => _collections; set { _collections = value; } }
        public string FilePath { get; set; } = string.Empty;
        public Result SaveCollections(string filePath)
        {
            try
            {
                using var writer = new BinaryWriter(File.Open(filePath, FileMode.Create));

                writer.Write(20250408);
                writer.Write(_collections.Count);

                foreach (var collection in _collections)
                {
                    WriteOsuString(writer, collection.Name);
                    writer.Write(collection.Hashes.Count);

                    foreach (string hash in collection.Hashes)
                    {
                        WriteOsuString(writer, hash);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure("Cannot save collections: " + ex.Message);
            }
            return Result.Success();

        }
        public Result LoadCollections()
        {
            try
            {
                var parseResult = ParseCollections();

                if (parseResult.IsFailure)
                    return Result.Failure("Cannot load collections.db file: " + parseResult.Error);

                _collections = parseResult.Value;
            }
            catch (Exception ex) { return Result.Failure("Cannot load collections.db file:" + ex.Message); }
            return Result.Success();
        }
        private Result<List<Collection>> ParseCollections()
        {
            List<Collection> collections = new List<Collection>();
            List<string> content = [];
            try
            {
                using StreamReader reader = new StreamReader(FilePath);

                var asd = reader.ReadToEnd();
                content = asd.Split('\v').ToList();
                
            }
            catch(Exception ex) { return Result.Failure<List<Collection>>(ex.Message);  }

            content.RemoveAt(0);

            int collectionIndex = -1;
            foreach (var line in content)
            {
                if (IsMD5(line.Trim()))
                {
                    collections[collectionIndex].Hashes.Add(line.Trim());
                }
                else
                {
                    collections.Add(new Collection()
                    {
                        Name = new string(line.Where(c => c > 31).ToArray()),
                        Hashes = []
                    });
                    collectionIndex++;
                }
            }

            return collections;
        }
        private bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
        private void WriteOsuString(BinaryWriter writer, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.Write((byte)0);
                return;
            }

            writer.Write((byte)11); 
            byte[] strBytes = Encoding.UTF8.GetBytes(value);
            WriteVarInt(writer, strBytes.Length);  
            writer.Write(strBytes);
        }
        private void WriteVarInt(BinaryWriter writer, int value)
        {
            while (true)
            {
                if (value < 0x80)
                {
                    writer.Write((byte)value);
                    return;
                }

                writer.Write((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
        }
    }
}
