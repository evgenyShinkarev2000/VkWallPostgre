namespace VkWallPostgre.Data
{
    public class ProcesedPost
    {
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// format "a5 b12 c23"
        /// </example>
        public string Tokens { get; set; } = default!;
        public string Domain { get; set; } = default!;
    }
}
