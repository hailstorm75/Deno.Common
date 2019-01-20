#pragma once
#include <vector>
#include <sstream>
#include <memory>
#define NAMEOF(x) std::string(#x)

namespace Common
{
  namespace Math
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

    constexpr Type operator | (const Type & selfValue, const Type & inValue)
    {
      return static_cast<Type>(unsigned(selfValue) | unsigned(inValue));
    }

    constexpr bool operator && (const Type & selfValue, const Type & inValue)
    {
      return (unsigned(selfValue) & unsigned(inValue)) != 0;
    }

    class InvertableMatrixOperationException : public std::runtime_error
    {
    public:
      explicit InvertableMatrixOperationException(const std::string & msg) noexcept : std::runtime_error(msg) {  }
    };

    class MatrixDimensionException : public std::runtime_error
    {
    public:
      explicit MatrixDimensionException(const std::string & msg) noexcept : std::runtime_error(msg) {  }
    };

    /**
     * \brief Class representing a mathematical matrix
     * \tparam T Type of matrix values. Type can only be numeric
     */
    template <typename T, typename = std::enable_if_t<std::is_arithmetic<T>::value && (std::is_floating_point<T>::value || std::is_integral<T>::value)>>
    class Matrix
    {
    protected:
      /**
       * \brief Type of given matrix
       */
      Type m_matrixType;
      /**
       * \brief  Determinant of the matrix.
       */
      double m_determinant = 0;
      /**
       * \brief Inverse of the matrix
       */
      std::shared_ptr<Matrix<T>> m_inverse;
      /**
       * \brief Matrix data
       */
      std::vector<std::vector<T>> m_matrixValues;

      void Recalculate()
      {
        m_determinant = CalculateDeterminant(m_matrixValues);
        m_inverse = CalculateInverse();
      }

      /**
       * \brief Validates collection
       * \param values Collection of values to validate
       * \return Bypassed collection
       */
      static const std::vector<std::vector<T>> & ValidateArray(const std::vector<std::vector<T>>& values)
      {
        if (values.empty())
          throw std::invalid_argument("Argument " + NAMEOF(values) + " cannot have a dimension of size 0.");
        auto colSize = values[0].size();
        for (const auto & value : values)
        {
          const auto tmp = value.size();
          if (tmp == 0)
            throw std::invalid_argument("Argument " + NAMEOF(values) + " cannot have a dimension of size 0.");
          if (tmp != colSize)
            throw std::invalid_argument("Argument " + NAMEOF(values) + " cannot be jagged.");
        }

        return values;
      }
      /**
       * \brief Initializes a new 2D vector instance
       * \param rows Number of rows in vector
       * \param cols Number of collumns in vector
       * \return Initialized vector
       */
      static std::vector<std::vector<T>> InitVector(const int rows, const int cols)
      {
        if (rows <= 0 || cols <= 0)
          throw std::invalid_argument("Size must be greater than 0.");

        std::vector<std::vector<T>> arr;
        arr.resize(rows);
        for (auto i = 0; i < rows; ++i)
          arr[i].resize(cols);

        return arr;
      }

      Matrix<T> CalculateMatrixOfCofactors() const noexcept
      {
        auto sign = 1;
        auto cofactorValues = InitVector(GetRows(), GetColumns());
        for (unsigned i = 0; i < GetColumns(); ++i)
        {
          for (unsigned j = 0; j < GetColumns(); ++j)
          {
            cofactorValues[i][j] = m_matrixValues[i][j] * sign;
            sign = -sign;
          }

          if (GetRows() % 2 == 0) sign = -sign;
        }

        return Matrix<T>(cofactorValues);
      }
      static Type CalculateMatrixType(const std::vector<std::vector<T>> & matrix)
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
      std::shared_ptr<Matrix<T>> CalculateInverse() const
      {
        if (m_matrixType && Type::NonInvertable)
          throw InvertableMatrixOperationException("Inverse can be calculated only for NxN matricies.");

        if (GetRows() == 2)
        {
          auto inverse = InitVector(2, 2);
          inverse[0][0] = m_matrixValues[1][1] * (1 / GetDeterminant());
          inverse[0][1] = m_matrixValues[0][1] * (1 / -GetDeterminant());
          inverse[1][0] = m_matrixValues[1][0] * (1 / -GetDeterminant());
          inverse[1][1] = m_matrixValues[0][0] * (1 / GetDeterminant());

          return std::make_shared<Matrix<T>>(inverse);
        }

        auto matrixofminors = InitVector(GetRows(), GetColumns());
        for (unsigned i = 0; i < GetColumns(); i++)
        {
          for (unsigned j = 0; j < GetColumns(); j++)
          {
            std::vector<unsigned> coords = { 0, 0 };
            auto submatrix = InitVector(GetRows() - 1, GetColumns() - 1);
            for (unsigned k = 0; k < GetColumns(); k++)
            {
              for (unsigned l = 0; l < GetColumns(); l++)
              {
                if (k == i || l == j) continue;
                submatrix[coords[0]][coords[1]++] = m_matrixValues[k][l];
              }

              if (coords[1] == GetColumns() - 1) coords[0]++;

              coords[1] = 0;
            }

            matrixofminors[i][j] = CalculateDeterminant(submatrix);
          }
        }

        auto minorMatrix = Matrix<T>(matrixofminors);
        std::vector<std::vector<T>> cofactorMatrix = minorMatrix.Transpose()->GetMatrixValues();

        for (unsigned i = 0; i < GetColumns(); i++)
          for (unsigned j = 0; j < GetColumns(); j++)
            cofactorMatrix[i][j] = static_cast<T>(cofactorMatrix[i][j] * 1 / GetDeterminant());

        return std::make_shared<Matrix<T>>(cofactorMatrix);
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

    public:
      explicit Matrix(const std::vector<std::vector<T>> & values)
        : m_matrixType(CalculateMatrixType(values)),
        m_matrixValues(ValidateArray(values)) { }
      Matrix(const int size, const bool identity)
        : m_matrixType(identity ? Type::Identity | Type::Invertable : Type::Invertable),
        m_matrixValues(InitVector(size, size))
      {
        if (identity)
          for (auto i = 0; i < size; ++i)
            m_matrixValues[i][i] = 1;
      }
      Matrix(const int length, const int height)
        : m_matrixType(length == height ? Type::Invertable : Type::NonInvertable),
        m_matrixValues(InitVector(length, height)) { }
      /**
       * \brief Copy constructor
       * \param matrix Matrix instance to copy from
       */
      explicit Matrix(const Matrix<T> & matrix)
        : m_matrixType(matrix.m_matrixType),
        m_determinant(matrix.m_determinant),
        m_inverse(matrix.m_inverse),
        m_matrixValues(matrix.m_matrixValues)
      { }
      /**
       * \brief Move constructor
       * \param other Matrix instance to move from
       */
      // explicit Matrix(Matrix<T> && other)
      //   : m_matrixType(std::move(other.m_matrixType)),
      //   m_determinant(std::move(other.m_determinant)),
      //   m_inverse(std::move(other.m_inverse)),
      //   m_matrixValues(std::move(other.m_matrixValues))
      // { }

      ~Matrix() = default;

      /**
       * \brief Getter method for the Rows property
       * \return Rows count
       */
      unsigned GetRows() const noexcept { return m_matrixValues.size(); }
      /**
       * \brief Getter method for the Columns property
       * \return Columns count
       */
      unsigned GetColumns() const noexcept { return m_matrixValues[0].size(); }
      /**
       * \brief Getter method for the MatrixValues property
       * \return Collection of matrix values
       */
      const std::vector<std::vector<T>> & GetMatrixValues() const noexcept { return m_matrixValues; }
      /**
       * \brief Setter method for the MatrixValues property
       * \param values Collection of values to set
       */
      void SetMatrixValues(const std::vector<std::vector<T>> & values)
      {
        m_matrixValues = ValidateArray(values);
        Recalculate();
      }
      /**
       * \brie Getter method for the Type property
       * \return Type of matrix
       */
      const Type & GetMatrixType() const noexcept { return m_matrixType; }
      /**
       * \brief Getter method for the Inverse property
       * \return Inverse of the matrix
       */
      const Matrix<T> & GetInverse() const noexcept { return m_inverse; }
      /**
       * \brief Getter method for the Determinant property
       * \return Determinant of the matrix
       */
      double GetDeterminant() const noexcept { return m_determinant; }
      Matrix<T> Transpose() const noexcept
      {
        auto transposedValues = InitVector(GetRows(), GetColumns());
        for (unsigned i = 0; i < GetColumns(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            transposedValues[i][j] = m_matrixValues[j][i];

        return Matrix<T>(transposedValues);
      }

      Matrix<T> & operator = (const Matrix<T> & other)
      {
        m_matrixValues = other.m_matrixValues;
        m_matrixType = other.m_matrixType;
        m_determinant = other.m_determinant;
        m_inverse = other.m_inverse;

        return *this;
      }
      // Matrix<T> & operator = (const Matrix<T> && other)
      // {
      //   m_matrixValues = std::move(other.m_matrixValues);
      //   m_matrixType = std::move(other.m_matrixType);
      //   m_determinant = std::move(other.m_determinant);
      //   m_inverse = std::move(other.m_inverse);

      //   return *this;
      // }

      Matrix<T> operator + (const Matrix<T> & other) const
      {
        if (GetRows() != other.GetRows() || GetColumns() != other.GetColumns())
          throw MatrixDimensionException("Matricies of different dimensions cannot be summed.");

        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), GetColumns());

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            outputValues[i][j] = m_matrixValues[i][j] + other.m_matrixValues[i][j];

        return Matrix<T>(outputValues);
      }
      Matrix<T> & operator +=(const Matrix<T> & other)
      {
        if (GetRows() != other.GetRows() || GetColumns() != other.GetColumns())
          throw MatrixDimensionException("Matricies of different dimensions cannot be summed.");

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            m_matrixValues[i][j] += other.m_matrixValues[i][j];

        return *this;
      }

      Matrix<T> operator - (const Matrix<T> & other) const
      {
        if (GetRows() != other.GetRows() || GetColumns() != other.GetColumns())
          throw MatrixDimensionException("Matricies of different dimensions cannot be summed.");

        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), GetColumns());

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            outputValues[i][j] = m_matrixValues[i][j] - other.m_matrixValues[i][j];

        return Matrix<T>(outputValues);
      }
      Matrix<T> & operator -=(const Matrix<T> & other)
      {
        if (GetRows() != other.GetRows() || GetColumns() != other.GetColumns())
          throw MatrixDimensionException("Matricies of different dimensions cannot be summed.");

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            m_matrixValues[i][j] -= other.m_matrixValues[i][j];

        return *this;
      }

      Matrix<T> operator * (const Matrix<T> & other) const
      {
        if (GetRows() != other.GetColumns())
          throw MatrixDimensionException("");

        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), other.GetColumns());

        for (unsigned i = 0; i < other.GetColumns(); i++)
          for (unsigned j = 0; j < GetRows(); j++)
            for (unsigned k = 0; k < GetRows(); k++)
              outputValues[i][j] = outputValues[i][j] + m_matrixValues[i][k] * other.m_matrixValues[k][j];

        return Matrix<T>(outputValues);
      }

      template <typename TOther, typename = std::enable_if_t<std::is_arithmetic<TOther>::value && (std::is_floating_point<TOther>::value || std::is_integral<TOther>::value)>>
      Matrix<T> operator * (const TOther & other) const
      {
        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), GetColumns());

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            outputValues[i][j] = m_matrixValues[i][j] * other;

        return Matrix<T>(outputValues);
      }
      template <typename TOther, typename = std::enable_if_t<std::is_arithmetic<TOther>::value && (std::is_floating_point<TOther>::value || std::is_integral<TOther>::value)>>
      Matrix<T> & operator *=(const TOther & other) const
      {
        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), GetColumns());

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            m_matrixValues[i][j] *= other;

        return *this;
      }

      template <typename TOther, typename = std::enable_if_t<std::is_arithmetic<TOther>::value && (std::is_floating_point<TOther>::value || std::is_integral<TOther>::value)>>
      Matrix<T> operator / (const TOther & other) const
      {
        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), GetColumns());

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            outputValues[i][j] = m_matrixValues[i][j] / other;

        return Matrix<T>(outputValues);
      }
      template <typename TOther, typename = std::enable_if_t<std::is_arithmetic<TOther>::value && (std::is_floating_point<TOther>::value || std::is_integral<TOther>::value)>>
      Matrix<T> & operator /=(const TOther & other) const
      {
        std::vector<std::vector<T>> outputValues = InitVector(GetRows(), GetColumns());

        for (unsigned i = 0; i < GetRows(); ++i)
          for (unsigned j = 0; j < GetColumns(); ++j)
            m_matrixValues[i][j] /= other;

        return *this;
      }

      std::string ToString() const
      {
        std::ostringstream result;
        for (unsigned row = 0; row < GetRows(); row++)
        {
          for (unsigned column = 0; column < GetColumns() - 1; column++)
            result << m_matrixValues[row][column];
          result << m_matrixValues[row][GetColumns() - 1];

          if (row != GetRows() - 1) result << '\n';
        }

        return result.str();
      }
    };
  }
}
