#pragma once
#include <vector>
#include <sstream>
#include <memory>
#define NAMEOF(x) std::string(#x)

namespace Common::Math
{
  /**
   * \brief Matrix properties
   */
  enum class Type : unsigned
  {
    /**
     * \brief N-by-M Matrix
     */
    NonInvertable = 1,
    /**
     * \brief N-by-N Matrix
     */
    Invertable = 2,
    /**
     * \brief Matrix with ones on the main diagonal and zeros elsewhere
     */
    Identity = 4
  };

  constexpr enum class Type operator | (const enum class Type selfValue, const enum class Type inValue)
  {
    return static_cast<enum class Type>(unsigned(selfValue) | unsigned(inValue));
  }

  constexpr bool operator && (const enum class Type selfValue, const enum class Type inValue)
  {
    return (unsigned(selfValue) & unsigned(inValue)) != 0;
  }

  class InvertableMatrixOperationException : public std::exception
  {
  public:
    InvertableMatrixOperationException() = default;
    explicit InvertableMatrixOperationException(char const * const msg) noexcept : std::exception(msg) {  }
    InvertableMatrixOperationException(char const * const msg, const int id) noexcept : std::exception(msg, id) {  }
  };

  class MatrixDimensionException : public std::exception
  {
  public:
    MatrixDimensionException() = default;
    explicit MatrixDimensionException(char const * const msg) noexcept : std::exception(msg) {  }
    MatrixDimensionException(char const * const msg, const int id) noexcept : std::exception(msg, id) {  }
  };

  /**
   * \brief Base class for all matricies
   * \tparam T Type of values of the matrix
   */
  template<class T>
  class BaseMatrix
  {
  protected:
    /**
     * \brief  Determinant of the matrix.
     */
    double m_determinant = 0;
    /**
     * \brief Inverse of the matrix
     */
    std::shared_ptr<BaseMatrix<T>> m_inverse;
    /**
     * \brief Matrix data
     */
    std::vector<std::vector<T>> m_matrixValues;

  public:
    explicit BaseMatrix(const std::vector<std::vector<T>> & matrix)
      : m_matrixValues(matrix) { }

    BaseMatrix() { throw std::exception("default used here"); }

    virtual ~BaseMatrix() = default;

    unsigned GetRows() const { return m_matrixValues.size(); }
    unsigned GetColumns() const { return m_matrixValues[0].size(); }

    virtual Type GetMatrixType() const = 0;
    virtual double GetDeterminant() const = 0;
    virtual std::shared_ptr<BaseMatrix<T>> Transpose() const noexcept = 0;
    virtual const std::vector<std::vector<T>> & GetMatrixValues() const noexcept = 0;
    virtual void SetMatrixValues(const std::vector<std::vector<T>> & values) = 0;

    std::string ToString() const
    {
      std::ostringstream result;
      for (auto row = 0; row < GetRows(); row++)
      {
        for (auto column = 0; column < GetColumns() - 1; column++)
          result << m_matrixValues[row][column];
        result << m_matrixValues[row][GetColumns() - 1];
        if (row != GetRows() - 1) result << '\n';
      }

      return result.str();
    }

  protected:
    virtual std::shared_ptr<BaseMatrix<T>> CalculateInverse() const = 0;
    virtual const std::shared_ptr<BaseMatrix<T>> & GetInverse() const = 0;
    virtual std::shared_ptr<BaseMatrix<T>> MatrixOfCofactors() const noexcept = 0;

    void Recalculate()
    {
      m_determinant = CalculateDeterminant(m_matrixValues);
      m_inverse = CalculateInverse();
    }

    static bool IsIdentity(const std::vector<std::vector<T>> & matrix)
    {
      unsigned index = 0;
      for (unsigned i = 0; i < matrix.size(); i++)
      {
        for (unsigned j = 0; j < matrix[0].size(); j++)
          if (matrix[i][j] != (index == j ? 1 : 0))
            return false;
        ++index;
      }

      return true;
    }

    static Type GetMatrixType(const std::vector<std::vector<T>> & matrix)
    {
      Type result;
      if (matrix.size() == matrix[0].size())
      {
        result = Type::Identity;
        if (IsIdentity(matrix)) result = result | Type::Identity;
      }
      else
        result = Type::NonInvertable;

      return result;
    }

    static double CalculateDeterminant(const std::vector<std::vector<T>> & matrix)
    {
      if (matrix.size() != matrix[0].size())
        throw InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      switch (matrix.size())
      {
      case 2:
        return static_cast<double>(matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0]);
      case 3:
      {
        auto left = matrix[0][0] * matrix[1][1] * matrix[2][2]
          + matrix[0][1] * matrix[1][2] * matrix[2][0]
          + matrix[0][2] * matrix[1][0] * matrix[2][1];
        auto right = matrix[0][2] * matrix[1][1] * matrix[2][0]
          + matrix[0][0] * matrix[1][2] * matrix[2][1]
          + matrix[0][1] * matrix[1][0] * matrix[2][2];
        return static_cast<double>(left - right);
      }
      default:
      {
        double determinant = 0;
        auto sign = 1;
        for (unsigned i = 0; i < matrix.size(); i++)
        {
          std::vector<unsigned> coords = { 0, 0 };
          std::vector<std::vector<T>> data = InitVector(matrix.size() - 1, matrix.size() - 1);

          for (unsigned row = 1; row < matrix.size(); row++)
          {
            for (unsigned col = 0; col < matrix.size(); col++)
            {
              if (row == 0 || col == i) continue;
              data[coords[0]][coords[1]++] = matrix[row][col];

              if (coords[1] != matrix.size() - 1) continue;
              coords[0]++;
              coords[1] = 0;
            }
          }

          determinant = matrix[0][i] * CalculateDeterminant(data) * sign + determinant;
          sign = -sign;
        }

        return determinant;
      }
      }
    }

    static std::vector<std::vector<T>> InitVector(const unsigned rows, const unsigned cols)
    {
      if (rows == 0 || cols == 0)
        throw std::invalid_argument("Size cannot be equal to 0.");

      std::vector<std::vector<T>> arr;
      arr.resize(rows);
      for (std::vector<T> item : arr)
        item.resize(cols);

      return arr;
    }
  };
}
