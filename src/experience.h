/*
  Hypnos, a private UCI chess playing engine with derived from Stockfish NNUE.
  with a sophisticated Self-Learning system implemented and control of evaluation strategies.
  
  1) Materialistic Evaluation Strategy: Minimum = -12, Maximum = +12, Default = 0.
  Lower values will cause the engine assign less value to material differences between the sides.
  More values will cause the engine to assign more value to the material difference.
  
  2) Positional Evaluation Strategy: Minimum = -12, Maximum = +12, Default = 0.
  Lower values will cause the engine assign less value to positional differences between the sides.
  More values will cause the engine to assign more value to the positional difference.
  
  The NNUE evaluation was first introduced in shogi, and ported to HypnoS afterward.
  It can be evaluated efficiently on CPUs, and exploits the fact that only parts of
  the neural network need to be updated after a typical chess move.
  
  The nodchip repository provides additional tools to train and develop the NNUE networks.
  Copyright (C) 2004-2022 The Hypnos developers (Marco Zerbinati)
*/

#ifndef __EXPERIENCE_H__
#define __EXPERIENCE_H__

#include "types.h"

using namespace std;

#define EXP_MIN_DEPTH ((Depth)4)

namespace Experience
{
    namespace V1
    {
        struct ExpEntry
        {
            Stockfish::Key     key;        //8 bytes
            Stockfish::Move    move;       //4 bytes
            Stockfish::Value   value;      //4 bytes
            Stockfish::Depth   depth;      //4 bytes
            uint8_t padding[4];            //4 bytes

            ExpEntry() = delete;
            ExpEntry(const ExpEntry& exp) = delete;
            ExpEntry& operator =(const ExpEntry& exp) = delete;

            explicit ExpEntry(Stockfish::Key k, Stockfish::Move m, Stockfish::Value v, Stockfish::Depth d)
            {
                key = k;
                move = m;
                value = v;
                depth = d;
                padding[0] = padding[2] = 0x00;
                padding[1] = padding[3] = 0xFF;
            }

            void merge(const ExpEntry* exp)
            {
                assert(key == exp->key);
                assert(move == exp->move);

                if (depth > exp->depth)
                    return;

                if (depth == exp->depth)
                {
                    value = (value + exp->value) / 2;
                }
                else
                {
                    value = exp->value;
                    depth = exp->depth;
                }
            }

            int compare(const ExpEntry* exp) const
            {
                int v = value * std::max(depth / 5, 1) - exp->value * std::max(exp->depth / 5, 1);
                if (!v)
                    v = depth - exp->depth;

                return v;
            }
        };

        static_assert(sizeof(ExpEntry) == 24);
    }

    namespace V2
    {
        struct ExpEntry
        {
            Stockfish::Key     key;        //8 bytes
            Stockfish::Move    move;       //4 bytes
            Stockfish::Value   value;      //4 bytes
            Stockfish::Depth   depth;      //4 bytes
            uint16_t           count;      //2 bytes (A scaled version of count)
            uint8_t padding[2];            //2 bytes

            ExpEntry() = delete;
            ExpEntry(const ExpEntry& exp) = delete;
            ExpEntry& operator =(const ExpEntry& exp) = delete;

            explicit ExpEntry(Stockfish::Key k, Stockfish::Move m, Stockfish::Value v, Stockfish::Depth d) : ExpEntry(k, m, v, d, 1) {}

            explicit ExpEntry(Stockfish::Key k, Stockfish::Move m, Stockfish::Value v, Stockfish::Depth d, uint16_t c)
            {
                key = k;
                move = m;
                value = v;
                depth = d;
                count = c;
                padding[0] = padding[1] = 0x00;
            }

            void merge(const ExpEntry* exp)
            {
                assert(key == exp->key);
                assert(move == exp->move);

                //Merge the count
                count = (uint16_t)std::min((uint32_t)count + (uint32_t)exp->count, (uint32_t)std::numeric_limits<uint16_t>::max());

                //Merge value and depth if 'exp' is better or equal
                if (depth > exp->depth)
                    return;

                if (depth == exp->depth)
                {
                    value = (value + exp->value) / 2;
                }
                else
                {
                    value = exp->value;
                    depth = exp->depth;
                }
            }

            int compare(const ExpEntry* exp) const
            {
                int v = value * std::max(depth / 10, 1) * std::max(count / 3, 1) - exp->value * std::max(exp->depth / 10, 1) * std::max(exp->count / 3, 1);
                if (v) return v;

                v = count - exp->count;
                if (v) return v;

                v = depth - exp->depth;
                return v;
            }
        };

        static_assert(sizeof(ExpEntry) == 24);
    }

    namespace Current = V2;

    //Experience structure
    struct ExpEntryEx : public Current::ExpEntry
    {
        ExpEntryEx* next = nullptr;

        ExpEntryEx() = delete;
        ExpEntryEx(const ExpEntryEx& exp) = delete;
        ExpEntryEx& operator =(const ExpEntryEx& exp) = delete;

        explicit ExpEntryEx(Stockfish::Key k, Stockfish::Move m, Stockfish::Value v, Stockfish::Depth d, uint8_t c) : Current::ExpEntry(k, m, v, d, c) {}

        ExpEntryEx* find(Stockfish::Move m) const
        {
            ExpEntryEx* exp = const_cast<ExpEntryEx*>(this);
            do
            {
                if (exp->move == m)
                    return exp;

                exp = exp->next;
            } while (exp);

            return nullptr;
        }

        ExpEntryEx* find(Stockfish::Move mv, Stockfish::Depth minDepth) const
        {
            ExpEntryEx* temp = const_cast<ExpEntryEx*>(this);
            do
            {
                if ((Stockfish::Move)temp->move == mv)
                {
                    if (temp->depth < minDepth)
                        temp = nullptr;

                    break;
                }

                temp = temp->next;
            } while (temp);

            return temp;
        }

        std::pair<int, bool> quality(Stockfish::Position& pos, int evalImportance) const;
    };
}

namespace Experience
{
    void init();
    bool enabled();

    void unload();
    void save();

    void wait_for_loading_finished();

    const ExpEntryEx* probe(Stockfish::Key k);

    void defrag(int argc, char* argv[]);
    void merge(int argc, char* argv[]);
    void show_exp(Stockfish::Position& pos, bool extended);
    void convert_compact_pgn(int argc, char* argv[]);

    void pause_learning();
    void resume_learning();
    bool is_learning_paused();

    void add_pv_experience(Stockfish::Key k, Stockfish::Move m, Stockfish::Value v, Stockfish::Depth d);
    void add_multipv_experience(Stockfish::Key k, Stockfish::Move m, Stockfish::Value v, Stockfish::Depth d);
}

#endif //__EXPERIENCE_H__

