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

#ifndef POLYBOOK_H_INCLUDED
#define POLYBOOK_H_INCLUDED

#include "bitboard.h"
#include "position.h"
#include "string.h"

typedef struct {
    uint64_t key;
    uint16_t move;
    uint16_t weight;
    uint32_t learn;
} PolyHash;

class PolyBook
{
public:

    PolyBook();
    ~PolyBook();

    void init(const std::string& bookfile);
    Stockfish::Move probe(Stockfish::Position& pos, bool bestBookMove);

private:

    Stockfish::Key polyglot_key(const Stockfish::Position& pos);
    Stockfish::Move pg_move_to_sf_move(const Stockfish::Position & pos, unsigned short pg_move);

    int find_first_key(uint64_t key);
    int get_key_data();

    bool check_draw(Stockfish::Position& pos, Stockfish::Move m);

    int keycount;
    PolyHash *polyhash;
    bool enabled;

    int index_first;
    int index_best;
    int index_rand;
    int index_count;
    int index_weight_count;
};

extern PolyBook polybook[2];

#endif // #ifndef POLYBOOK_H_INCLUDED