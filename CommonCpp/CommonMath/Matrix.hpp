#pragma once
#include "BaseMatrix.hpp"

template <class T>
class Matrix : public BaseMatrix<T>
{
  Type m_matrixType;

public:
  explicit Matrix(const std::vector<std::vector<T>> & values)
    : m_matrixType(GetMatrixType(values))
  {
  }

  Matrix(const int size, const bool identity)
    : m_matrixType(identity ? Type::Identity | Type::Invertable : Type::Invertable)
  {
  }

  Matrix(const int length, const int height)
    : m_matrixType(length == height ? Type::Invertable : Type::NonInvertable)
  {

  }

  const std::shared_ptr<BaseMatrix<T>> & Transpose() const override
  {

  }
};
