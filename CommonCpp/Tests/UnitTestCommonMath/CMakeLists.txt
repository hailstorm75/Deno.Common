cmake_minimum_required(VERSION 3.0)

set(CMAKE_BUILD_TYPE Test)
set(CMAKE_CXX_FLAGS "-std=gnu++17")

project(UnitTestCommonMath)

add_executable(
  UnitTestCommonMath
  main.cpp
  UtNumberInRange.cpp
)
