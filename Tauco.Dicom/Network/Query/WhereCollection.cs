using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Utilities.Extensions;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Collection containing constraints used for filtering requested items.
    /// </summary>
    /// <typeparam name="TInfo">The type of elements contained in the collection.</typeparam>
    internal class WhereCollection<TInfo> : List<WhereItem>, IWhereCollection<TInfo> where TInfo : IDicomInfo
    {
        private readonly IDicomMapping mDicomMapping;


        /// <summary>
        /// Gets predicate matching all condition within the collection usable for LINQ expressions.
        /// </summary>
        public Func<TInfo, bool> Predicate
        {
            get
            {
                var param = Expression.Parameter(typeof (TInfo), "c");
                Expression body = null;

                foreach (var property in mDicomMapping.Where(item => ContainsTag(item.Value)))
                {
                    var whereItem = this[property.Value];
                    Expression propertyExpression = null;

                    foreach (var item in whereItem)
                    {
                        Expression itemExpression;

                        switch (item.Operator)
                        {
                            case WhereOperator.Equals:
                                itemExpression = GetEqualsExpression(param, property, item);
                                break;

                            case WhereOperator.Like:
                                itemExpression = CallLikeExpression(param, property, item);
                                break;

                            default:
                                throw new NotSupportedException();
                        }

                        propertyExpression = propertyExpression == null ? itemExpression : Expression.OrElse(propertyExpression, itemExpression);
                    }

                    if (propertyExpression != null)
                    {
                        body = body == null ? propertyExpression : Expression.AndAlso(body, propertyExpression);
                    }
                }

                if (body == null)
                {
                    return _ => true;
                }

                return Expression.Lambda<Func<TInfo, bool>>(body, param).Compile();
            }
        }


        /// <summary>
        /// Instantiates new instance of <see cref="WhereCollection{TInfo}" />.
        /// </summary>
        /// <param name="dicomMapping">Provides mapping from DICOM properties to ObjectInfo properties</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomMapping" /> is <see langword="null" /></exception>
        public WhereCollection([NotNull] IDicomMapping dicomMapping)
        {
            if (dicomMapping == null)
            {
                throw new ArgumentNullException(nameof(dicomMapping));
            }

            mDicomMapping = dicomMapping;
        }


        /// <summary>
        /// Indexer returning collection of <see cref="WhereItem" /> according to the given <paramref name="dicomTag" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which <see cref="WhereItem" /> should be selected</param>
        /// <exception cref="ArgumentException"><paramref name="dicomTag" /> equals <see cref="DicomTags.Undefined" /></exception>
        /// <returns>Instance of <see cref="WhereItem" /> corresponding to the given <paramref name="dicomTag" /></returns>
        public IEnumerable<WhereItem> this[DicomTags dicomTag]
        {
            get
            {
                if (dicomTag == DicomTags.Undefined)
                {
                    throw new ArgumentException("Tag cannot be undefined", nameof(dicomTag));
                }

                return this.Where(item => item.DicomTag == dicomTag);
            }
        }


        /// <summary>
        /// Adds new like condition to the collection. Will match all objects with given <paramref name="dicomTag" /> containing
        /// given <paramref name="value" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which property will be used for filering</param>
        /// <param name="value">Value needed to be contained within the property value</param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentException"><paramref name="dicomTag" /> equals <see cref="DicomTags.Undefined" /></exception>
        public void WhereLike(DicomTags dicomTag, [NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException("Tag cannot be undefined", nameof(dicomTag));
            }

            Add(dicomTag, value, WhereOperator.Like);
        }


        /// <summary>
        /// Adds new like condition to the collection. Will match all objects with given <paramref name="dicomTag" /> which equals
        /// to given <paramref name="value" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which property will be used for filering</param>
        /// <param name="value">Value needed to equal to the property value</param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentException"><paramref name="dicomTag" /> equals <see cref="DicomTags.Undefined" /></exception>
        public void WhereEquals(DicomTags dicomTag, [NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException("Tag cannot be undefined", nameof(dicomTag));
            }

            Add(dicomTag, value);
        }


        /// <summary>
        /// Determines whether the collection contains the given <paramref name="dicomTag" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag the request is made for</param>
        /// <exception cref="ArgumentException"><paramref name="dicomTag" /> equals <see cref="DicomTags.Undefined" /></exception>
        /// <returns>True, if collection contains <paramref name="dicomTag" />; otherwise, false</returns>
        public bool ContainsTag(DicomTags dicomTag)
        {
            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException("Tag cannot be undefined", nameof(dicomTag));
            }

            return this.Any(item => item.DicomTag == dicomTag);
        }


        /// <summary>
        /// Gets all where collections suitable for DICOM C-FIND request created from current <see cref="WhereCollection{TInfo}"/>.
        /// New combination has to be created for every OR statement within the collection (when one <see cref="DicomTags"/> has specified more than one value).
        /// </summary>
        /// <returns>Collection containing all combinations obtained from current <see cref="WhereCollection{TInfo}"/></returns>
        public IEnumerable<IDicomWhereCollection> GetDicomWhereCollections()
        {
            var input = this
                .GroupBy(c => c.DicomTag)
                .Select(a => this[a.Key].Cast<object>().ToList())
                .ToList();

            foreach (var combination in GetCombinations(input))
            {
                var dicomWhereCollection = new DicomWhereCollection();
                dicomWhereCollection.AddRange(combination.Cast<WhereItem>().Select(whereItem => new DicomWhereItem
                {
                    Value = GetDicomCompatibleConditions(whereItem),
                    DicomTag = whereItem.DicomTag
                }));

                yield return dicomWhereCollection;
            }
        }


        /// <summary>
        /// Get all combinations of items in given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Collection of collections containing values to be used for creating combinations</param>
        /// <returns>All combinations of <see cref="value"/></returns>
        private IEnumerable<IEnumerable<object>> GetCombinations(List<List<object>> value)
        {
            if (!value.Any())
            {
                return new[]
                {
                    Enumerable.Empty<object>()
                };
            }

            var query =
                from x in value.First()
                from y in GetCombinations(value.Skip(1).ToList())
                select new[] { x }.Concat(y);
            return query;
        }

        /// <summary>
        /// Gets collection of SQL like syntax condition for given <paramref name="whereItem" />, which can be used in DICOM data retrieval.
        /// </summary>
        /// <param name="whereItem">Where item for which is SQL like syntax codition supposed to be get for</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="whereItem" /> <see cref="WhereItem.DicomTag"/> equals <see cref="DicomTags.Undefined" /> -or- unable
        /// to get result for given <paramref name="whereItem" />
        /// </exception>
        /// <returns>SQL like syntax condition suitable for DICOM data retrieving</returns>
        private string GetDicomCompatibleConditions(WhereItem whereItem)
        {
            switch (whereItem.Operator)
            {
                case WhereOperator.Equals:
                    return whereItem.Value.ToString();
                     
                case WhereOperator.Like:
                    return $"*{whereItem.Value}*";
                        
                default:
                    throw new ArgumentException($"Unable to get the result for given {whereItem}");
            }
        }


        /// <summary>
        /// Gets proper like expression for given <paramref name="item"/>.
        /// </summary>
        /// <param name="param">Base parameter expression</param>
        /// <param name="item">Dicom mapping item</param>
        /// <param name="whereItem">Represents single constraint item</param>
        /// <returns>Like comparison expression for given <paramref name="item"/></returns>
        private Expression CallLikeExpression(ParameterExpression param, KeyValuePair<PropertyInfo, DicomTags> item, WhereItem whereItem)
        {
            var containsMethod = item.Key.PropertyType.GetMethod("Contains");
            if (containsMethod != null)
            {
                return Expression.Call(Expression.PropertyOrField(param, item.Key.Name), containsMethod,
                    Expression.Constant(whereItem.Value.ToString()));
            }

            var methodInfo = typeof (StringExtensions).GetMethod("Contains",
                BindingFlags.Public | BindingFlags.Static);

            var arguments = new List<Expression>
            {
                GetToStringExpression(Expression.PropertyOrField(param, item.Key.Name), Expression.Default(item.Key.PropertyType)),
                GetToStringExpression(Expression.Constant(whereItem.Value), Expression.Default(whereItem.Value.GetType())),
                Expression.Constant(CultureInfo.CurrentCulture)
            };

            return Expression.Call(methodInfo, arguments);
        }


        /// <summary>
        /// Gets proper equals expression for given <paramref name="item"/>.
        /// </summary>
        /// <param name="param">Base parameter expression</param>
        /// <param name="item">Dicom mapping item</param>
        /// <param name="whereItem">Represents single constraint item</param>
        /// <returns>Equal comparison expression for given <paramref name="item"/></returns>
        private MethodCallExpression GetEqualsExpression(ParameterExpression param, KeyValuePair<PropertyInfo, DicomTags> item, WhereItem whereItem)
        {
            var equalsMethod = GetEqualsMethod(item.Key);
            return Expression.Call(Expression.PropertyOrField(param, item.Key.Name), equalsMethod,
                Expression.Constant(whereItem.Value));
        }


        /// <summary>
        /// Gets proper Equals method for given <paramref name="property"/> type.
        /// </summary>
        /// <param name="property">Property specifying type for which is the method obtained</param>
        /// <returns>Equals method of <paramref name="property"/> type accepting either <see cref="System.String"/> or <see cref="System.Object"/> as parameter</returns>
        private static MethodInfo GetEqualsMethod(PropertyInfo property)
        {
            return property.PropertyType.GetMethod("Equals", new[] { property.PropertyType }) ??
                property.PropertyType.GetMethod("Equals", new[] { typeof (object) });
        }


        /// <summary>
        /// Build expression for given <paramref name="property" />. If <paramref name="property" /> is null, returns
        /// <paramref name="defaultExpression" />.
        /// </summary>
        /// <param name="property">Proeprty the condition is built for</param>
        /// <param name="defaultExpression">
        /// Defaul value returned if <paramref name="property" /> is <see langword="null" />
        /// </param>
        /// <returns>
        /// Expression returning either result of ToString method, or <paramref name="defaultExpression" /> is
        /// <paramref name="property" /> is null
        /// </returns>
        private Expression GetToStringExpression(Expression property, DefaultExpression defaultExpression)
        {
            var isNull = Expression.Equal(property, defaultExpression);
            return Expression.Condition(isNull, Expression.Constant(string.Empty),
                Expression.Call(property, "ToString", null));
        }


        /// <summary>
        /// Adds condition with for given arguments.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which property will be used for the filering</param>
        /// <param name="value">Value used for the filering</param>
        /// <param name="operator">
        /// Operator specifying binary relation between <paramref name="dicomTag" /> property value and
        /// <paramref name="value" />
        /// </param>
        private void Add(DicomTags dicomTag, object value, WhereOperator @operator = WhereOperator.Equals)
        {
            if (ContainsTag(dicomTag))
            {
                if (this[dicomTag].Any(item => item.Value == value && item.Operator == @operator))
                {
                    return;
                }
            }
            
            Add(new WhereItem
            {
                DicomTag = dicomTag,
                Value = value,
                Operator = @operator
            });
        }
    }
}