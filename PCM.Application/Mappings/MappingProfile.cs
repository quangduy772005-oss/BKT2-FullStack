using AutoMapper;
using PCM.Application.DTOs;
using PCM.Core.Entities;

namespace PCM.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Identity
        CreateMap<AppUser, UserDto>();
        
        // Member
        CreateMap<Member, MemberDto>();
        CreateMap<UpdateMemberProfileRequest, Member>();

        // Courts & Bookings
        CreateMap<Court, CourtDto>();
        CreateMap<Booking, BookingDto>()
            .ForMember(d => d.CourtName, opt => opt.MapFrom(s => s.Court.Name))
            .ForMember(d => d.MemberName, opt => opt.MapFrom(s => s.Member.FullName));
        CreateMap<CreateBookingRequest, Booking>();

        // Wallet
        CreateMap<WalletTransaction, WalletTransactionDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<DepositRequest, DepositRequestDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // Tournament
        CreateMap<Tournament, TournamentDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Format, opt => opt.MapFrom(s => s.Format.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ParticipantCount, opt => opt.MapFrom(s => s.Participants.Count));
        CreateMap<CreateTournamentRequest, Tournament>();
        
        CreateMap<TournamentParticipant, TournamentParticipantDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.MemberName, opt => opt.MapFrom(s => s.Member.FullName));

        // Match
        CreateMap<Match, MatchDto>()
            .ForMember(d => d.MatchFormat, opt => opt.MapFrom(s => s.MatchFormat.ToString()))
            .ForMember(d => d.Team1Player1, opt => opt.MapFrom(s => s.Team1Player1!.FullName))
            .ForMember(d => d.Team1Player2, opt => opt.MapFrom(s => s.Team1Player2!.FullName))
            .ForMember(d => d.Team2Player1, opt => opt.MapFrom(s => s.Team2Player1!.FullName))
            .ForMember(d => d.Team2Player2, opt => opt.MapFrom(s => s.Team2Player2!.FullName))
            .ForMember(d => d.Result, opt => opt.MapFrom(s => s.Result.ToString()));

        // News
        CreateMap<News, NewsDto>();
    }
}
