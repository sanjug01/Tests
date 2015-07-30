using System;
using System.Collections.Generic;

namespace RdClient.Shared.Helpers
{
    public class MemoizeCodeBlock<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _codeBlock;
        private IDictionary<TIn, TOut> _cache;
        private uint _maxCacheCount;

        public MemoizeCodeBlock(Func<TIn, TOut> codeBlock, uint maxCacheCount = 50)
        {
            _codeBlock = codeBlock;
            _cache = new Dictionary<TIn, TOut>();
            _maxCacheCount = maxCacheCount;
        }

        public TOut GetValue(TIn param)
        {
            TOut result;
            if(false == _cache.TryGetValue(param, out result))
            {
                if (_cache.Count > _maxCacheCount)
                    _cache.Clear();

                result = _codeBlock(param);
                _cache.Add(param, result);
            }

            return result;
        }
    }
}
