using System;
using System.Collections.Generic;

namespace MmdAssetMethods.Extension
{
    /// <summary>
    /// IList / IEnumerable に対するユーティリティ拡張メソッド群。
    /// Move は指定したアイテムをリスト内の指定インデックスに移動（存在しなければ挿入）します。
    /// Select は選択時の副作用を行いつつ要素を変換して列挙します。
    /// </summary>
    internal static class ListControl
    {
        /// <summary>
        /// 指定したアイテムをリストの指定インデックス位置へ移動します。
        /// <para>既にリスト内に存在する場合は最初の出現を削除してから挿入します（移動）。</para>
        /// <para>アイテムが存在しない場合は指定位置へ挿入します。</para>
        /// <para>index が範囲外の場合は安全にクリップ（0..list.Count）されます。</para>
        /// </summary>
        /// <typeparam name="T">リスト要素の型（参照型）</typeparam>
        /// <param name="list">移動対象のリスト（null の場合は何もしない）</param>
        /// <param name="index">挿入先インデックス（負なら 0、list.Count より大きければ list.Count に丸められる）</param>
        /// <param name="item">移動/挿入する項目（null の場合は何もしない）</param>
        public static void Move<T>(this IList<T> list, int index, T item) where T : class
        {
            // 互換性のため null リスト／null 要素は単に何もしない（元実装と同様）
            if (list == null) return;
            if (item == null) return;

            // 空リストなら単純に挿入（index をクリップ）
            if (list.Count == 0)
            {
                var insertIndex = index < 0 ? 0 : (index > 0 ? 0 : index);
                // 上は index が 0 以外なら 0 にする形。簡潔に扱うため以下で確実に 0 にする。
                list.Insert(0, item);
                return;
            }

            // index を安全な範囲にクリップ（挿入は list.Count を許容して append できるように）
            if (index < 0) index = 0;
            if (index > list.Count) index = list.Count;

            // 現在の位置を探す（存在しないなら -1）
            int currentIndex = list.IndexOf(item);

            // 既にその位置にいるなら何もしない
            if (currentIndex == index) return;

            if (currentIndex >= 0)
            {
                // 既にリストに存在する -> 削除してから挿入
                // RemoveAt を使うことで確実に最初の出現を取り除く
                list.RemoveAt(currentIndex);

                // 削除した位置が挿入先より前だった場合、挿入先インデックスは 1 減る
                if (currentIndex < index) index--;
            }

            // 再度安全クリップ（念のため）
            if (index < 0) index = 0;
            if (index > list.Count) index = list.Count;

            list.Insert(index, item);
        }

        /// <summary>
        /// 要素を列挙しながら副作用(onSelected)を実行し、selector によって変換された結果列を返します。
        /// </summary>
        /// <typeparam name="TSource">入力シーケンスの要素型</typeparam>
        /// <typeparam name="TResult">出力シーケンスの要素型</typeparam>
        /// <param name="source">入力シーケンス（null の場合は例外）</param>
        /// <param name="onSelected">各要素に対して先に呼ばれる副作用アクション（null 禁止）</param>
        /// <param name="selector">各要素を変換して返す関数（null 禁止）</param>
        /// <returns>副作用を行った後に selector で変換された要素列</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Action<TSource> onSelected, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (onSelected == null) throw new ArgumentNullException(nameof(onSelected));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            // 注意: ここで System.Linq.Enumerable.Select を明示的に呼ぶことで
            // 自身の拡張メソッドへの再帰呼び出しを避けます。
            return System.Linq.Enumerable.Select(source, item =>
            {
                onSelected(item);
                return selector(item);
            });
        }
    }
}
