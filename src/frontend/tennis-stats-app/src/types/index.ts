// Player types
export interface Player {
  id: number
  externalId: number
  firstName: string
  lastName: string
  fullName: string
  country?: string
  dateOfBirth?: string
  age?: number
  heightCm?: number
  weightKg?: number
  hand: string
  backhand: string
  turnedProYear?: number
  imageUrl?: string
  association: string
  isActive: boolean
  currentRank?: number
  currentPoints?: number
}

export interface PlayerDetail extends Player {
  rankingHistory: RankingHistory[]
  currentSeasonStats?: PlayerSeasonStats
}

export interface RankingHistory {
  rank: number
  points: number
  rankChange?: number
  rankingDate: string
}

export interface PlayerSeasonStats {
  year: number
  matchesPlayed: number
  matchesWon: number
  matchesLost: number
  winPercentage: number
  titlesWon: number
}

// Ranking types
export interface Ranking {
  id: number
  playerId: number
  playerName?: string
  country?: string
  rank: number
  points: number
  previousRank?: number
  rankChange?: number
  rankingDate: string
  association: string
}

// Tournament types
export interface Tournament {
  id: number
  externalId: number
  name: string
  city?: string
  country?: string
  location?: string
  surface: string
  startDate?: string
  endDate?: string
  prizeMoney?: number
  currency?: string
  association: string
  category?: string
  isCompleted: boolean
}

// Match types
export interface Match {
  id: number
  externalId: number
  scheduledAt?: string
  scheduledDate?: string
  startedAt?: string
  endedAt?: string
  round: string
  status: string
  durationMinutes?: number
  player1Id: number
  player1Name?: string
  player1?: Player
  player1Score?: number
  player2Id: number
  player2Name?: string
  player2?: Player
  player2Score?: number
  winnerId?: number
  score?: string
  tournament?: Tournament
  tournamentId?: number
  sets?: SetScore[]
}

export interface MatchDetail extends Match {
  tournamentId: number
  tournamentName?: string
  sets: SetScore[]
  statistics?: MatchStatistics
}

export interface SetScore {
  setNumber: number
  player1Games: number
  player2Games: number
  tiebreakPlayer1Points?: number
  tiebreakPlayer2Points?: number
  isTiebreak: boolean
}

export interface MatchStatistics {
  player1Aces?: number
  player1DoubleFaults?: number
  player1FirstServePercentage?: number
  player1Winners?: number
  player1UnforcedErrors?: number
  player2Aces?: number
  player2DoubleFaults?: number
  player2FirstServePercentage?: number
  player2Winners?: number
  player2UnforcedErrors?: number
}

// Pagination types
export interface PaginatedList<T> {
  items: T[]
  pageNumber: number
  totalPages: number
  totalCount: number
  pageSize: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

// API response types
export interface ApiError {
  error: string
  errorCode?: string
}

// Association enum
export enum Association {
  WTA = 'WTA',
  ATP = 'ATP',
}
