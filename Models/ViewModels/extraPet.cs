using FurryFriendFinder.Models.Data;
using System;
using System.Collections.Generic;
namespace FurryFriendFinder.Models.ViewModels;


public class extraPet
{

public StateHealth stateHealth { get; set; } = null!;
 public Vaccine vaccine { get; set; } = null!;
 public Pet pet { get; set; } = null!;

public AnimalType animalType { get; set; } = null!;
public Breed breed { get; set; } = null!;

    public extraPet(Pet pet)
    {
        this.pet = pet;
    }


}
