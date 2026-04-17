#!/usr/bin/env bash
set -u -o pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_FILE="$ROOT_DIR/FundamentalAlgorithms.slnx"
TREE_TESTS_PROJECT="$ROOT_DIR/TreeDataStructures.Tests/TreeDataStructures.Tests.csproj"
ARITH_TESTS_PROJECT="$ROOT_DIR/Arithmetic.Tests/Arithmetic.Tests.csproj"

resolve_dotnet() {
  if [[ -n "${DOTNET_BIN:-}" ]] && [[ -x "$DOTNET_BIN" ]]; then
    echo "$DOTNET_BIN"
    return 0
  fi

  if [[ -x "/tmp/.dotnet/dotnet" ]]; then
    echo "/tmp/.dotnet/dotnet"
    return 0
  fi

  if command -v dotnet >/dev/null 2>&1; then
    command -v dotnet
    return 0
  fi

  return 1
}

run_dotnet_test() {
  local target="$1"
  local filter="${2:-}"

  if [[ ! -f "$target" ]]; then
    echo "Не найден файл: $target"
    return 1
  fi

  if [[ -n "$filter" ]]; then
    "$DOTNET" test "$target" --filter "$filter"
  else
    "$DOTNET" test "$target"
  fi
}

if ! DOTNET="$(resolve_dotnet)"; then
  echo "dotnet не найден."
  echo "Установи .NET SDK или задай путь: DOTNET_BIN=/path/to/dotnet ./run_tests_menu.sh"
  exit 1
fi

while true
 do
  echo
  echo "Выбери тесты для запуска:"
  echo "1) TreeDataStructures: все"
  echo "2) TreeDataStructures: все кроме Treap"
  echo "3) TreeDataStructures: BST"
  echo "4) TreeDataStructures: AVL"
  echo "5) TreeDataStructures: RB"
  echo "6) TreeDataStructures: Splay"
  echo "7) TreeDataStructures: Treap"
  echo "8) Arithmetic: все"
  echo "9) Arithmetic: Base"
  echo "10) Arithmetic: Bitwise"
  echo "11) Arithmetic: MultiplicationSimple"
  echo "12) Arithmetic: MultiplicationKaratsuba"
  echo "13) Arithmetic: MultiplicationFFT"
  echo "14) Все тесты (solution)"
  echo "0) выход"
  read -r -p "Введите номер: " choice

  if [[ "$choice" == "0" ]]; then
    exit 0
  fi

  if [[ "$choice" == "1" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT"
  elif [[ "$choice" == "2" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT" "Category!=Treap"
  elif [[ "$choice" == "3" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT" "Category=BST"
  elif [[ "$choice" == "4" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT" "Category=AVL"
  elif [[ "$choice" == "5" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT" "Category=RB"
  elif [[ "$choice" == "6" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT" "Category=Splay"
  elif [[ "$choice" == "7" ]]; then
    run_dotnet_test "$TREE_TESTS_PROJECT" "Category=Treap"
  elif [[ "$choice" == "8" ]]; then
    run_dotnet_test "$ARITH_TESTS_PROJECT"
  elif [[ "$choice" == "9" ]]; then
    run_dotnet_test "$ARITH_TESTS_PROJECT" "Category=Base"
  elif [[ "$choice" == "10" ]]; then
    run_dotnet_test "$ARITH_TESTS_PROJECT" "Category=Bitwise"
  elif [[ "$choice" == "11" ]]; then
    run_dotnet_test "$ARITH_TESTS_PROJECT" "Category=MultiplicationSimple"
  elif [[ "$choice" == "12" ]]; then
    run_dotnet_test "$ARITH_TESTS_PROJECT" "Category=MultiplicationKaratsuba"
  elif [[ "$choice" == "13" ]]; then
    run_dotnet_test "$ARITH_TESTS_PROJECT" "Category=MultiplicationFFT"
  elif [[ "$choice" == "14" ]]; then
    run_dotnet_test "$SOLUTION_FILE"
  else
    echo "Неверный номер. Повтори ввод."
  fi
 done
