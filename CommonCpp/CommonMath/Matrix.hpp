#pragma once
#include "BaseMatrix.hpp"

namespace Common::Math
{
  template <class T, std::enable_if_t<std::is_arithmetic<T>::value && std::is_integral<T>::value>* = nullptr>
  class Matrix : public BaseMatrix<T>
  {
    Type m_matrixType;

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

  public:
    explicit Matrix(const std::vector<std::vector<T>> & values)
      : BaseMatrix<T>(ValidateArray(values)),
      m_matrixType(BaseMatrix<T>::GetMatrixType(values))
    {
    }

    Matrix(const int size, const bool identity)
      : BaseMatrix<T>(BaseMatrix<T>::InitVector(size, size)),
      m_matrixType(identity ? Type::Identity | Type::Invertable : Type::Invertable)
    {
      if (identity)
        for (auto i = 0; i < size; ++i)
          BaseMatrix<T>::m_matrixValues[i][i] == 1;
    }

    Matrix(const int length, const int height)
      : BaseMatrix<T>(BaseMatrix<T>::InitVector(length, height)),
      m_matrixType(length == height ? Type::Invertable : Type::NonInvertable)
    {
    }

    Type GetMatrixType() const override
    {
      return m_matrixType;
    }
    double GetDeterminant() const override
    {
      return BaseMatrix<T>::m_determinant;
    }
    std::shared_ptr<BaseMatrix<T>> Transpose() const noexcept override
    {
      auto transposedValues = BaseMatrix<T>::InitVector(BaseMatrix<T>::GetRows(), BaseMatrix<T>::GetColumns());
      for (unsigned i = 0; i < BaseMatrix<T>::GetColumns(); ++i)
        for (unsigned j = 0; j < BaseMatrix<T>::GetColumns(); ++j)
          transposedValues[i][j] = BaseMatrix<T>::m_matrixValues[j][i];

      return std::make_shared<Matrix<T>>(transposedValues);
    }
    const std::vector<std::vector<T>> & GetMatrixValues() const noexcept override
    {
      return BaseMatrix<T>::m_matrixValues;
    }
    void SetMatrixValues(const std::vector<std::vector<T>> & values) override
    {
      BaseMatrix<T>::m_matrixValues = values;
      BaseMatrix<T>::Recalculate();
    }

  protected:
    std::shared_ptr<BaseMatrix<T>> CalculateInverse() const override
    {
      if (m_matrixType && Type::NonInvertable)
        throw InvertableMatrixOperationException("Inverse can be calculated only for NxN matricies.");

      if (BaseMatrix<T>::GetRows() == 2)
      {
        auto inverse = BaseMatrix<T>::InitVector(2, 2);
        inverse[0][0] = BaseMatrix<T>::m_matrixValues[1][1] * (1 / GetDeterminant());
        inverse[0][1] = BaseMatrix<T>::m_matrixValues[0][1] * (1 / -GetDeterminant());
        inverse[1][0] = BaseMatrix<T>::m_matrixValues[1][0] * (1 / -GetDeterminant());
        inverse[1][1] = BaseMatrix<T>::m_matrixValues[0][0] * (1 / GetDeterminant());

        return std::make_shared<Matrix<T>>(inverse);
      }

      auto matrixofminors = BaseMatrix<T>::InitVector(BaseMatrix<T>::GetRows(), BaseMatrix<T>::GetColumns());
      for (unsigned i = 0; i < BaseMatrix<T>::GetColumns(); i++)
      {
        for (unsigned j = 0; j < BaseMatrix<T>::GetColumns(); j++)
        {
          std::vector<unsigned> coords = { 0, 0 };
          auto submatrix = BaseMatrix<T>::InitVector(BaseMatrix<T>::GetRows() - 1, BaseMatrix<T>::GetColumns() - 1);
          for (unsigned k = 0; k < BaseMatrix<T>::GetColumns(); k++)
          {
            for (unsigned l = 0; l < BaseMatrix<T>::GetColumns(); l++)
            {
              if (k == i || l == j) continue;
              submatrix[coords[0]][coords[1]++] = BaseMatrix<T>::m_matrixValues[k][l];
            }

            if (coords[1] == BaseMatrix<T>::GetColumns() - 1) coords[0]++;

            coords[1] = 0;
          }

          matrixofminors[i][j] = BaseMatrix<T>::CalculateDeterminant(submatrix);
        }
      }

      auto minorMatrix = Matrix<T>(matrixofminors);
      std::vector<std::vector<T>> cofactorMatrix = minorMatrix.Transpose()->GetMatrixValues();

      for (unsigned i = 0; i < BaseMatrix<T>::GetColumns(); i++)
        for (unsigned j = 0; j < BaseMatrix<T>::GetColumns(); j++)
          cofactorMatrix[i][j] = static_cast<T>(cofactorMatrix[i][j] * 1 / GetDeterminant());

      return std::make_shared<Matrix<T>>(cofactorMatrix);
    }
    const std::shared_ptr<BaseMatrix<T>> & GetInverse() const override
    {
      return BaseMatrix<T>::m_inverse;
    }
    std::shared_ptr<BaseMatrix<T>> MatrixOfCofactors() const noexcept override
    {
      auto sign = 1;
      auto cofactorValues = BaseMatrix<T>::InitVector(BaseMatrix<T>::GetRows(), BaseMatrix<T>::GetColumns());
      for (unsigned i = 0; i < BaseMatrix<T>::GetColumns(); ++i)
      {
        for (unsigned j = 0; j < BaseMatrix<T>::GetColumns(); ++j)
        {
          cofactorValues[i][j] = BaseMatrix<T>::m_matrixValues[i][j] * sign;
          sign = -sign;
        }

        if (BaseMatrix<T>::GetRows() % 2 == 0) sign = -sign;
      }

      return std::make_shared<Matrix<T>>(cofactorValues);
    }
  };
}
