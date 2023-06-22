namespace ElecPOE.DTO
{
    [Serializable]
    public class JsonResponseDTO
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
    }
}
